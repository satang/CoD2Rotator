using CoDRotator.Properties;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private string GAME_PATH;
        private string CUSTOM_MAPS;
        private string MAPS_PATH;
        private string CONFIG_FILE;

        private ZipFile zipfile;

        public Form1(bool isServer = false)
        {
            InitializeComponent();

            string game = Settings.Default.GamePath;

            if (string.IsNullOrWhiteSpace(game) || !Directory.Exists(game) || !File.Exists(Path.Combine(game, "CoD2MP_s.exe")))
            {
                var dialog = new FolderBrowserDialog();
                DialogResult result;
                do
                {
                    result = dialog.ShowDialog();
                } while (result != System.Windows.Forms.DialogResult.OK);

                Settings.Default.GamePath = dialog.SelectedPath;
                Settings.Default.Save();

                game = dialog.SelectedPath;
            }

            GAME_PATH = game;
            MAPS_PATH = Path.Combine(GAME_PATH, "main");
            CUSTOM_MAPS = Path.Combine(MAPS_PATH, "customs");
            CONFIG_FILE = Path.Combine(MAPS_PATH, "server_configs\\server.cfg");

            if (!Directory.Exists(CUSTOM_MAPS))
            {
                MessageBox.Show("No tenes la carpeta con todos los mapas en: " + CUSTOM_MAPS);
            }

            if (!isServer)
            {
                this.Visible = false;
                LaunchClient();
                this.Shown += (sender, args) => { this.Close(); };
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(CUSTOM_MAPS, "*.iwd", SearchOption.TopDirectoryOnly);

            listBox1.Items.Add(new MyListItem
            {
                Name = "Del juego",
                Tag = "GameOriginals"
            });

            foreach (string file in files)
            {
                listBox1.Items.Add(new MyListItem
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Tag = file
                });
            }
        }

        private bool IsGameMaps()
        {
            var item = listBox1.SelectedItem as MyListItem;
            if (item == null)
                return false;

            return (string)item.Tag == "GameOriginals";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                listBox2.Items.Clear();
                pictureBox1.Image = null;

                if (zipfile != null)
                {
                    zipfile.Dispose();
                    zipfile = null;
                }

                var selected = (sender as ListBox).SelectedItem as MyListItem;

                if (IsGameMaps())
                {
                    zipfile = null;
                    RefreshGameMaps(GetSelectedGameType());
                }
                else
                {
                    zipfile = new ZipFile((string)selected.Tag);

                    RefreshMaps(GetSelectedGameType());
                }
            }
            catch (Exception)
            {
                listBox2.Items.Add(new MyListItem
                {
                    Name = "Error",
                    Tag = null
                });
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            var gameType = rb.Tag as string;
            if (IsGameMaps())
                RefreshGameMaps(gameType);
            else
                RefreshMaps(gameType);
        }

        private void RefreshGameMaps(string gameType)
        {
            try
            {
                listBox2.Items.Clear();
                pictureBox1.Image = null;

                listBox2.Items.Add(new MyListItem { Name = "Villers Bocage", Tag = "mp_breakout" });
                listBox2.Items.Add(new MyListItem { Name = "St. Mere Eglise", Tag = "mp_dawnville" });
                listBox2.Items.Add(new MyListItem { Name = "Carentan", Tag = "mp_carentan" });
                listBox2.Items.Add(new MyListItem { Name = "Burgundy", Tag = "mp_burgundy" });
                listBox2.Items.Add(new MyListItem { Name = "Brecourt", Tag = "mp_brecourt" });
                listBox2.Items.Add(new MyListItem { Name = "Beltot", Tag = "mp_farmhouse" });
                listBox2.Items.Add(new MyListItem { Name = "Caen", Tag = "mp_trainstation" });
                listBox2.Items.Add(new MyListItem { Name = "El Alamein", Tag = "mp_decoy" });
                listBox2.Items.Add(new MyListItem { Name = "Leningrad", Tag = "mp_leningrad" });
                listBox2.Items.Add(new MyListItem { Name = "Matmata", Tag = "mp_matmata" });
                listBox2.Items.Add(new MyListItem { Name = "Moscow", Tag = "mp_downtown" });
                listBox2.Items.Add(new MyListItem { Name = "Stalingrad", Tag = "mp_railyard" });
                listBox2.Items.Add(new MyListItem { Name = "Toujane", Tag = "mp_toujane" });
                listBox2.Items.Add(new MyListItem { Name = "Rostov", Tag = "mp_harbor" });
                listBox2.Items.Add(new MyListItem { Name = "Wallendar", Tag = "mp_rhine" });
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void RefreshMaps(string gameType)
        {
            try
            {
                listBox2.Items.Clear();
                pictureBox1.Image = null;

                if (zipfile != null)
                {
                    var entries = zipfile.Entries.Where(x => x.FileName.StartsWith(@"mp/") && x.FileName.EndsWith(".arena"));

                    HashSet<string> gameTypes = new HashSet<string>();

                    foreach (var file in entries)
                    {
                        MemoryStream myStream = new MemoryStream();
                        file.Extract(myStream);
                        myStream.Position = 0;
                        var reader = new StreamReader(myStream);
                        var data = reader.ReadToEnd();

                        var mapGamesTypes = GetGameTypes(data);

                        if (mapGamesTypes.Contains(gameType))
                        {
                            listBox2.Items.Add(new MyListItem
                            {
                                Name = GetMapName(data),
                                Tag = file
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Image = null;
                RefreshMapInfo();

                if (!IsGameMaps())
                {

                    if (pictureBox1.Image == null)
                    {
                        var selected = (sender as ListBox).SelectedItem as MyListItem;

                        string name = selected.Name;

                        var imageFileEntry = zipfile.Entries.Where(x => x.FileName.StartsWith("images/") &&
                            x.FileName.EndsWith(".dds") &&
                            x.FileName.Contains(name));

                        var p = zipfile.Entries.Where(x => x.FileName.EndsWith("dds"));

                        if (imageFileEntry.Any())
                        {
                            var destFile = System.IO.Path.GetTempFileName().Replace(".tmp", ".dds");
                            FileStream fs = new FileStream(destFile, FileMode.OpenOrCreate);
                            imageFileEntry.First().Extract(fs);
                            fs.Flush();
                            fs.Close();

                            LoadDDS(destFile);
                        }
                        else
                        {
                            imageFileEntry = zipfile.Entries.Where(x => x.FileName.StartsWith("images/") &&
                            x.FileName.EndsWith(".iwi") &&
                            x.FileName.Contains(name));

                            if (imageFileEntry.Any())
                            {
                                var destFile = System.IO.Path.GetTempFileName().Replace(".tmp", ".iwi");
                                FileStream fs = new FileStream(destFile, FileMode.OpenOrCreate);
                                imageFileEntry.First().Extract(fs);
                                fs.Flush();
                                fs.Close();

                                LoadIWI(destFile);

                                File.Delete(destFile);
                            }
                        }

                        SaveCurrentMap();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private string GetCurrentPackage()
        {
            string package = (string)((MyListItem)listBox1.SelectedItem).Name;
            return package;
        }

        private string GetCurrentMapName()
        {
            string package = (string)((MyListItem)listBox2.SelectedItem).Name;
            return package;
        }

        private void RefreshMapInfo()
        {
            string package = GetCurrentPackage();
            string name = GetCurrentMapName();
            MapInfo map = MapsInfo.GetMap(package, name);

            if (map != null)
            {
                equipo1.Text = map.Team1;
                equipo2.Text = map.Team2;
                puntaje.Text = map.Score.ToString();
                comentario.Text = map.Description;
                pictureBox1.Image = map.Thumbnail;
            }
            else
            {
                equipo1.Text = null;
                equipo2.Text = null;
                puntaje.Text = null;
                comentario.Text = null;
            }
        }

        private void LoadIWI(string destFile)
        {
            try
            {
                Process iwi2dds = new Process();
                iwi2dds.StartInfo.CreateNoWindow = true;
                iwi2dds.StartInfo.FileName = @"IWI_X_DDS.exe";
                iwi2dds.StartInfo.Arguments = destFile;
                iwi2dds.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                iwi2dds.Start();
                while (iwi2dds.MainWindowHandle == default(System.IntPtr))
                    Thread.Sleep(1);

                iwi2dds.Kill();

                var ddsFile = destFile.Replace(".iwi", ".dds");

                LoadDDS(ddsFile);

                File.Delete(ddsFile);
            }
            catch (Exception)
            {
            }
        }

        private void LoadDDS(string destFile)
        {
            try
            {
                var destFileTransformed = destFile.Replace(".dds", ".jpg");
                var img = new ImageMagickNET.Image();
                img.Read(destFile);
                img.Write(destFileTransformed);
                img.Dispose();

                FileStream newfs = new FileStream(destFileTransformed, FileMode.Open, FileAccess.Read);
                pictureBox1.Image = Image.FromStream(newfs);
                newfs.Close();
                File.Delete(destFileTransformed);
            }
            catch (Exception)
            {
            }
        }

        private string GetMapName(string data)
        {
            var start = data.IndexOf('"', data.IndexOf("map") + 1) + 1;
            var end = data.IndexOf('"', start);
            var result = data.Substring(start, end - start);
            return result;
        }

        private List<string> GetGameTypes(string data)
        {
            var result = new List<string>();
            var start = data.IndexOf('"', data.IndexOf("gametype") + 1) + 1;
            var end = data.IndexOf('"', start);
            var intermediate = data.Substring(start, end - start);

            result = intermediate.Split(' ').ToList();

            return result.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            var selected = (sender as ListBox).SelectedItem as MyListItem;

            var mapName = (IsGameMaps() ? selected.Tag : selected.Name) as string;
            var gameType = "";

            gameType = GetSelectedGameType();

            listBox3.Items.Add(new MyListItem
            {
                Name = gameType + " -> " + mapName,
                Tag = new KeyValuePair<string, string>(gameType, mapName)
            });
        }

        private string GetSelectedGameType()
        {
            foreach (var item in radioPanel.Controls.OfType<RadioButton>())
                if (item.Checked)
                    return item.Tag as string;
            return "";
        }

        private void listBox3_DoubleClick(object sender, EventArgs e)
        {
            var listbox = (sender as ListBox);

            if (listbox.SelectedIndex >= 0)
                listbox.Items.RemoveAt(listbox.SelectedIndex);
        }

        private void LaunchClient()
        {
            CleanAndCopyMaps(MapsInfo.GetCurrentPack());

            Process.Start(new ProcessStartInfo
            {
                Arguments = "",
                CreateNoWindow = true,
                FileName = Path.Combine(GAME_PATH, "CoD2MP_s.exe"),
                UseShellExecute = true,
                WorkingDirectory = GAME_PATH
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var items = listBox3.Items.OfType<MyListItem>();
                if (items.Any())
                {
                    var lastGameType = "";
                    string result = "";
                    foreach (var item in items)
                    {
                        KeyValuePair<string, string> current = (KeyValuePair<string, string>)item.Tag;
                        if (current.Key != lastGameType)
                        {
                            lastGameType = current.Key;
                            result += "gametype " + lastGameType + " ";
                        }

                        result += "map " + current.Value + " ";
                    }

                    result = "set sv_mapRotation \"" + result + "\"";

                    string content = MapsInfo.GetOriginalConfig();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        using (StreamReader source = new StreamReader("Cod2_server.cfg"))
                        {
                            content = source.ReadToEnd();
                        }
                    }

                    content = content.Replace("set sv_hostname \"Andresito\"", "set sv_hostname \"" + GetCurrentPackage() + "\"");

                    content += Environment.NewLine + Environment.NewLine + "// MAPS" + Environment.NewLine + Environment.NewLine + result;

                    using (TextWriter dest = new StreamWriter(CONFIG_FILE, false))
                    {
                        dest.Write(content);
                        dest.Flush();
                        dest.Close();
                    }

                    CleanAndCopyMaps(GetCurrentPackage());

                    MapsInfo.SaveCurrentPackageConfig(SystemInformation.ComputerName, GetCurrentPackage());

                    MessageBox.Show("Todo listo: A JUGAR!!!", "COD2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    Process.Start(new ProcessStartInfo
                    {
                        Arguments = "+exec server_configs\\server.cfg +map_rotate ",
                        CreateNoWindow = true,
                        FileName = Path.Combine(GAME_PATH, "CoD2MP_s.exe"),
                        UseShellExecute = true,
                        WorkingDirectory = GAME_PATH
                    });
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void CleanAndCopyMaps(string pack)
        {
            var existingMaps = Directory.EnumerateFiles(MAPS_PATH, "mapas*");

            bool alreadyHaveMap = false;
            foreach (var item in existingMaps)
            {
                var file = new FileInfo(item);
                if (file.Name != pack + ".iwd")
                {
                    file.Delete();
                }
                else
                    alreadyHaveMap = true;
            }

            if (!alreadyHaveMap)
            {
                existingMaps = Directory.EnumerateFiles(CUSTOM_MAPS, "mapas*");
                FileInfo selectedFileName = null;

                foreach (var item in existingMaps)
                {
                    var file = new FileInfo(item);
                    if (file.Name == pack + ".iwd")
                    {
                        selectedFileName = file;
                        break;
                    }
                }

                if (selectedFileName != null)
                {
                    selectedFileName.CopyTo(Path.Combine(MAPS_PATH, selectedFileName.Name));
                }
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "COD2", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void comentario_Validated(object sender, EventArgs e)
        {
            SaveCurrentMap();
        }

        private void SaveCurrentMap()
        {
            int score;
            if (!int.TryParse(puntaje.Text, out score))
                score = int.MinValue;

            string package = GetCurrentPackage();
            string name = GetCurrentMapName();

            MapsInfo.SaveMap(package, name, equipo1.Text, equipo2.Text, score, comentario.Text, pictureBox1.Image);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var results = MapsInfo.SearchMaps(textBox1.Text);
            listBox4.DataSource = results;
        }
    }

    class MyListItem
    {
        public string Name { get; set; }

        public object Tag { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }


    public class MapInfo
    {
        public int MapInfoId { get; set; }

        public string Package { get; set; }
        public string Name { get; set; }

        public string Team1 { get; set; }
        public string Team2 { get; set; }

        public int Score { get; set; }

        public string Description { get; set; }

        public byte[] Image { get; set; }

        [NotMapped]
        public Image Thumbnail
        {
            get
            {
                if (Image == null)
                    return null;

                using (var stream = new MemoryStream(Image))
                {
                    return System.Drawing.Image.FromStream(stream);
                }
            }
            set
            {
                if (value != null)
                {
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        Bitmap bm = new Bitmap(value);
                        bm.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        Image = memoryStream.ToArray();
                    }
                }
            }
        }
    }

    public class ServerSetting
    {
        public int ServerSettingId { get; set; }

        public string Computer { get; set; }

        public string Package { get; set; }

        public DateTime PackageDate { get; set; }
    }

    public class CoDRotator : DbContext
    {
        public DbSet<MapInfo> Maps { get; set; }

        public DbSet<ServerSetting> ServerSettings { get; set; }
    }

    public static class MapsInfo
    {
        public static MapInfo GetMap(string package, string name)
        {
            using (var ct = new CoDRotator())
            {
                return ct.Maps.FirstOrDefault(x => x.Package == package && x.Name == name);
            }
        }

        public static void SaveMap(string package, string name, string team1, string team2, int score, string description, Image image)
        {
            using (var ct = new CoDRotator())
            {
                var map = ct.Maps.FirstOrDefault(x => x.Package == package && x.Name == name);

                if (map == null)
                {
                    map = new MapInfo
                    {
                        Package = package,
                        Name = name
                    };

                    ct.Maps.Add(map);
                }

                if (map.Team1 != team1 || map.Team2 != team2 || (map.Score != score) || map.Description != description || map.Thumbnail != image)
                {
                    map.Thumbnail = image;
                    map.Team1 = team1;
                    map.Team2 = team2;

                    if (score != int.MinValue)
                        map.Score = score;

                    map.Description = description;
                    ct.SaveChanges();
                }
            }
        }

        public static List<string> SearchMaps(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            using (var ct = new CoDRotator())
            {
                var splits = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                return ct.Maps
                    .OrderByDescending(x => x.Score)
                    .Where(x =>
                        splits.All(y => x.Description.Contains(y)
                        || x.Name.Contains(y)))
                    .Select(x => x.Package + " -> " + x.Name)
                    .ToList();
            }
        }

        public static void SaveCurrentPackageConfig(string computer, string pack)
        {
            using (var ct = new CoDRotator())
            {
                var config = ct.ServerSettings.FirstOrDefault(x => x.Computer == computer);
                if (config == null)
                {
                    config = new ServerSetting();
                    config.Computer = computer;
                    ct.ServerSettings.Add(config);
                }

                config.Package = pack;
                config.PackageDate = DateTime.Now;

                ct.SaveChanges();
            }
        }

        public static string GetOriginalConfig()
        {
            using (var ct = new CoDRotator())
            {
                var config = ct.ServerSettings.FirstOrDefault(x => x.Computer == "CONFIG");

                if (config == null)
                    return null;

                return config.Package;
            }
        }

        public static string GetCurrentPack()
        {
            using (var ct = new CoDRotator())
            {
                return ct.ServerSettings.OrderByDescending(x => x.PackageDate).Select(x => x.Package).FirstOrDefault();
            }
        }
    }
}
