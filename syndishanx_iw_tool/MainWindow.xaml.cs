using System.Windows;
using WindowsInput;
using WindowsInput.Native;
using Memory;
using System.Diagnostics;
using System.Security.Policy;
using System.IO;
using System.Text;

namespace syndishanx_iw_tool {
    public partial class MainWindow : Window {
        public Mem Memlib = new();

        InputSimulator input = new();

        public string processName = "";
        public string addressBase = "";

        public bool infiniteAmmo = false;
        public bool infiniteGrenades = false;
        public bool godMode = false;
        public bool noClip = false;
        public bool noTarget = false;
        public bool noRecoil = false;
        public bool rapidFire = false;
        public bool thirdPerson = false;
        public bool oneHitKill = false;
        public bool lowGravity = false;

        // Zombies XP - Lvl 1000 | Memlib.WriteMemory("base+5ECA68C", "int", "99999999");
        // MP - Prestige 30 | Memlib.WriteMemory("base+5EC3ECA", "int", "30");
        // Spawn Special Zombies | Memlib.WriteMemory(addressBase + "6854DC0", "int", "2");
        // Unlock FPS |  Memlib.WriteMemory("base+75D0A30", "bytes", "E8 03 00 00 38 51 E3 C3 D0 98 31 4D 01 00 00 00 E8 03 00 00 38");
        // Relock FPS |  Memlib.WriteMemory("base+75D0A30", "bytes", "55 00 00 00 85 52 E3 C3 01 00 00 00 00 00 00 00 55 00 00 00 85");
        // Challenge Exploit | Memlib.WriteMemory("base+5ECB661", "bytes", " 79 195 0 0 41 2 79 195 0 0 41 2");
        // Reset Contract | Memlib.WriteMemory("base+5EC5BDD", "int", "0");
        // Swap Right Contract (1-40) | Memlib.WriteMemory("base+5EC5BD7", "int", "0");

        public MainWindow() {
            InitializeComponent();

            Infinite_Ammo_Checkbox.IsEnabled = false;
            Infinite_Grenades_Checkbox.IsEnabled = false;
            Godmode_Checkbox.IsEnabled = false;
            No_Clip_Checkbox.IsEnabled = false;
            No_Target_Checkbox.IsEnabled = false;
            No_Recoil_Checkbox.IsEnabled = false;
            Rapid_Fire_Checkbox.IsEnabled = false;
            Third_Person_Checkbox.IsEnabled = false;
            One_Hit_Kill_Checkbox.IsEnabled = false;
            Low_Gravity_Checkbox.IsEnabled = false;

            Hotkeys_Checkbox.IsEnabled = false;

            Timescale_TextBox.IsEnabled = false;
            Set_Timescale_Button.IsEnabled = false;
            Scene_TextBox.IsEnabled = false;
            Set_Scene_Button.IsEnabled = false;
            Currency_Combo_Box.IsEnabled = false;
            Freeze_Currency_Checkbox.IsEnabled = false;
            Currency_TextBox.IsEnabled = false;
            Set_Currency_Button.IsEnabled = false;
            Log_TextBox.IsEnabled = false;

            ConnectToClient(false);
        }

        private void ConnectToClient(bool wasClicked) {
            try {
                if (Memlib.OpenProcess("iw7_ship")) {
                    processName = "iw7_ship";
                    addressBase = "base+0x";
                }
                else if (Memlib.OpenProcess("iw7-mod")) {
                    processName = "iw7-mod";
                    addressBase = "14";
                } else if (wasClicked) {
                    MessageBox.Show("Failed to Connect to Process");
                }
                if (processName != "") {
                    Connect_Button.IsEnabled = false;
                    Connect_Button.Content = "Connected to " + processName + ".exe";

                    Infinite_Ammo_Checkbox.IsEnabled = true;
                    Infinite_Grenades_Checkbox.IsEnabled = true;
                    Godmode_Checkbox.IsEnabled = true;
                    No_Clip_Checkbox.IsEnabled = true;
                    No_Target_Checkbox.IsEnabled = true;
                    No_Recoil_Checkbox.IsEnabled = true;
                    Rapid_Fire_Checkbox.IsEnabled = true;
                    Third_Person_Checkbox.IsEnabled = true;
                    One_Hit_Kill_Checkbox.IsEnabled = true;
                    Low_Gravity_Checkbox.IsEnabled = true;

                    Hotkeys_Checkbox.IsEnabled = true;

                    Timescale_TextBox.IsEnabled = true;
                    Set_Timescale_Button.IsEnabled = true;
                    Scene_TextBox.IsEnabled = true;
                    Set_Scene_Button.IsEnabled = true;
                    Currency_Combo_Box.IsEnabled = true;
                    Freeze_Currency_Checkbox.IsEnabled = true;
                    Currency_TextBox.IsEnabled = true;
                    Set_Currency_Button.IsEnabled = true;

                    Hotkeys_Checkbox.IsChecked = true;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Failed to Connect to Process");
            }
        }

        private void Connect_Button_Click(object sender, RoutedEventArgs e) {
            ConnectToClient(true);
        }

        private async void Infinite_Ammo_Checked(object sender, RoutedEventArgs e) {
            infiniteAmmo = !infiniteAmmo;

            IEnumerable<long> AoBScanResults = await Memlib.AoBScan(0x140000000, 0x150000000, "01 7C 90 14 48 8B 4C 24 60", false, true);
            long SingleAoBScanResult = AoBScanResults.FirstOrDefault();

            if (infiniteAmmo) {
                Memlib.WriteMemory(SingleAoBScanResult.ToString("X"), "bytes", "90 90 90 90");
                Log_TextBox.Text = "Infinite Ammo Enabled";
            } else if (!infiniteAmmo) {
                Memlib.WriteMemory(SingleAoBScanResult.ToString("X"), "bytes", "01 7C 90 14 48 8B 4C 24 60");
                Log_TextBox.Text = "Infinite Ammo Disabled";
            }
        }

        private void Infinite_Grenades_Checked(object sender, RoutedEventArgs e) {
            infiniteGrenades = !infiniteGrenades;

            var infiniteGrenadesValue = infiniteGrenades ? "4" : "0";

            if (infiniteGrenadesValue == "4") {
                // Primary Grenades Count (Starts from 2)
                Memlib.FreezeValue(addressBase + "3CA2AE4", "int", infiniteGrenadesValue);
                // Primary Grenades HUD Count (Starts from 2)
                Memlib.FreezeValue(addressBase + "68BC3F8", "int", infiniteGrenadesValue);
                Log_TextBox.Text = "Infinite Grenades Enabled";
            } else if (infiniteGrenadesValue == "0") {
                Memlib.UnfreezeValue(addressBase + "3CA2AE4");
                Memlib.UnfreezeValue(addressBase + "68BC3F8");
                Log_TextBox.Text = "Infinite Grenades Disabled";
            }
        }

        private void Godmode_Checked(object sender, RoutedEventArgs e) {
            godMode = !godMode;
            
            var godModeValue = godMode ? "10000" : "100";

            if (godModeValue == "10000") {
                if (noTarget) {
                    Memlib.UnfreezeValue(addressBase + "3D227F0");
                }
                Memlib.WriteMemory(addressBase + "3D227F0", "int", "10000");
                Log_TextBox.Text = "God Mode Enabled";
            } else if (godModeValue == "100") {
                Memlib.UnfreezeValue(addressBase + "3D227F0");
                Memlib.WriteMemory(addressBase + "3D227F0", "int", "100");
                Log_TextBox.Text = "God Mode Disabled";
            }
        }

        private void No_Clip_Checked(object sender, RoutedEventArgs e) {
            noClip = !noClip;

            var noClipValue = noClip ? "2" : "0";

            Memlib.WriteMemory(addressBase + "3CA6AF4", "byte", noClipValue);

            if (noClipValue == "2") {
                Log_TextBox.Text = "No Clip Enabled";
            } else if (noClipValue == "0") {
                Log_TextBox.Text = "No Clip Disabled";
            }
        }

        private void No_Target_Checked(object sender, RoutedEventArgs e) {
            noTarget = !noTarget;
        
            var noTargetValue = noTarget ? "FF 7F C6 A4" : "100";
        
            if (noTargetValue == "FF 7F C6 A4") {
                if (godMode) {
                    Memlib.UnfreezeValue(addressBase + "3D227F0");
                }
                Memlib.FreezeValue(addressBase + "3D227F0", "bytes", "FF 7F C6 A4");
                Log_TextBox.Text = "No Target Enabled";
            } else if (noTargetValue == "100") {
                Memlib.UnfreezeValue(addressBase + "3D227F0");
                Memlib.WriteMemory(addressBase + "3D227F0", "int", noTargetValue);
                Log_TextBox.Text = "No Target Disabled";
            }
        }

        private void No_Recoil_Checked(object sender, RoutedEventArgs e) {
            noRecoil = !noRecoil;
            
            var noRecoilValue = noRecoil ? "11437327" : "11437071";
            
            Memlib.WriteMemory(addressBase + "8E188C", "int", noRecoilValue);
            
            if (noRecoilValue == "11437327") {
                Log_TextBox.Text = "No Recoil Enabled";
            } else if (noRecoilValue == "11437071") {
                Log_TextBox.Text = "No Recoil Disabled";
            }
        }

        private void Rapid_Fire_Checked(object sender, RoutedEventArgs e) {
            rapidFire = !rapidFire;

            if (rapidFire) {
                Memlib.WriteBytes(addressBase + "072EB61", new byte[1] { (byte) 148 });
                Log_TextBox.Text = "Rapid Fire Enabled";
            } else if (!rapidFire) {
                Memlib.WriteBytes(addressBase + "072EB61", new byte[1] { (byte) 132 });
                Log_TextBox.Text = "Rapid Fire Disabled";
            }
        }

        private void Third_Person_Checked(object sender, RoutedEventArgs e) {
            thirdPerson = !thirdPerson;

            var thirdPersonValue = thirdPerson ? "1" : "0";

            Memlib.WriteMemory("1475D76F0", "int", thirdPersonValue);

            if (thirdPersonValue == "1") {
                Log_TextBox.Text = "Third Person Enabled";
            } else if (thirdPersonValue == "0") {
                Log_TextBox.Text = "Third Person Disabled";
            }
        }

        private void One_Hit_Kill_Checked(object sender, RoutedEventArgs e) {
            oneHitKill = !oneHitKill;

            if (oneHitKill) {
                Memlib.FreezeValue(addressBase + "3D26F60", "int", "1");
                Memlib.FreezeValue(addressBase + "3D27358", "int", "1");
                Memlib.FreezeValue(addressBase + "3D27750", "int", "1");
                Memlib.FreezeValue(addressBase + "3D27B48", "int", "1");
                Memlib.FreezeValue(addressBase + "3D28B28", "int", "1");
                Memlib.FreezeValue(addressBase + "3D28730", "int", "1");
                Memlib.FreezeValue(addressBase + "3D28338", "int", "1");
                Memlib.FreezeValue(addressBase + "3D27F40", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2AEE0", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2BAC8", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2BEC0", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2C6B0", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2C2B8", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2CAA8", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2B6D0", "int", "1");
                Memlib.FreezeValue(addressBase + "3D28F20", "int", "1");
                Memlib.FreezeValue(addressBase + "3D29318", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2B2D8", "int", "1");
                Memlib.FreezeValue(addressBase + "3D29710", "int", "1");
                Memlib.FreezeValue(addressBase + "3D29B08", "int", "1");
                Memlib.FreezeValue(addressBase + "3D29F00", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2A2F8", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2A6F0", "int", "1");
                Memlib.FreezeValue(addressBase + "3D2AAE8", "int", "1");
                Log_TextBox.Text = "One Hit Kill Enabled";
            } else if (!oneHitKill) {
                Memlib.UnfreezeValue(addressBase + "3D26F60");
                Memlib.UnfreezeValue(addressBase + "3D27358");
                Memlib.UnfreezeValue(addressBase + "3D27750");
                Memlib.UnfreezeValue(addressBase + "3D27B48");
                Memlib.UnfreezeValue(addressBase + "3D28B28");
                Memlib.UnfreezeValue(addressBase + "3D28730");
                Memlib.UnfreezeValue(addressBase + "3D28338");
                Memlib.UnfreezeValue(addressBase + "3D27F40");
                Memlib.UnfreezeValue(addressBase + "3D2AEE0");
                Memlib.UnfreezeValue(addressBase + "3D2BAC8");
                Memlib.UnfreezeValue(addressBase + "3D2BEC0");
                Memlib.UnfreezeValue(addressBase + "3D2C6B0");
                Memlib.UnfreezeValue(addressBase + "3D2C2B8");
                Memlib.UnfreezeValue(addressBase + "3D2CAA8");
                Memlib.UnfreezeValue(addressBase + "3D2B6D0");
                Memlib.UnfreezeValue(addressBase + "3D28F20");
                Memlib.UnfreezeValue(addressBase + "3D29318");
                Memlib.UnfreezeValue(addressBase + "3D2B2D8");
                Memlib.UnfreezeValue(addressBase + "3D29710");
                Memlib.UnfreezeValue(addressBase + "3D29B08");
                Memlib.UnfreezeValue(addressBase + "3D29F00");
                Memlib.UnfreezeValue(addressBase + "3D2A2F8");
                Memlib.UnfreezeValue(addressBase + "3D2A6F0");
                Memlib.UnfreezeValue(addressBase + "3D2AAE8");
                Log_TextBox.Text = "One Hit Kill Disabled";
            }
        }

        private void Low_Gravity_Checked(object sender, RoutedEventArgs e) {
            lowGravity = !lowGravity;

            if (lowGravity) {
                Memlib.WriteBytes(addressBase + "070F47C", new byte[1] { (byte)117 });
                Log_TextBox.Text = "Low Gravity Enabled";
            } else if (!lowGravity) {
                Memlib.WriteBytes(addressBase + "070F47C", new byte[1] { (byte)116 });
                Log_TextBox.Text = "Low Gravity Disabled";
            }
        }

        private void Set_Timescale_Button_Click(object sender, RoutedEventArgs e) {
            var setTimescaleValue = Timescale_TextBox.Text;

            Memlib.WriteMemory(addressBase + "6005D84", "float", setTimescaleValue);
            Log_TextBox.Text = "Timescale set to " + setTimescaleValue;
        }

        private void Set_Scene_Button_Click(object sender, RoutedEventArgs e) {
            var setSceneValue = Scene_TextBox.Text;

            Memlib.WriteMemory(addressBase + "6854DC0", "bytes", setSceneValue);
            Log_TextBox.Text = "Scene set to " + setSceneValue;
        }

        private void Complete_Contracts_Button_Click(object sender, RoutedEventArgs e) {
            Memlib.WriteMemory(addressBase + "5ECB661", "bytes", "4F C3 00 00 29 02 4F C3 00 00 29 02");
            Log_TextBox.Text = "Contracts Completed!";
        }

        private void Reset_Contracts_Button_Click(object sender, RoutedEventArgs e) {
            Memlib.WriteMemory(addressBase + "5ECB66D", "bytes", "67");
            Log_TextBox.Text = "Contracts Reset!";
        }

        private void Hotkeys_Checked(object sender, RoutedEventArgs e) {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Timer_Tick);
            dispatcherTimer.Interval = new TimeSpan(50);

            if (Hotkeys_Checkbox.IsChecked == true) {
                dispatcherTimer.Start();
            } else {
                dispatcherTimer.Stop();
            }
        }

        private void Timer_Tick(object? sender, EventArgs e) {
            if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD0)) {
                No_Clip_Checkbox.IsChecked = !No_Clip_Checkbox.IsChecked;
                Thread.Sleep(500);
            } else if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD1)) {
                Infinite_Ammo_Checkbox.IsChecked = !Infinite_Ammo_Checkbox.IsChecked;
                Thread.Sleep(500);
            } else if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD2)) {
                Godmode_Checkbox.IsChecked = !Godmode_Checkbox.IsChecked;
                Thread.Sleep(500);
            } else if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD3)) {
                No_Target_Checkbox.IsChecked = !No_Target_Checkbox.IsChecked;
                Thread.Sleep(500);
            } else if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD4)) {
                No_Recoil_Checkbox.IsChecked = !No_Recoil_Checkbox.IsChecked;
                Thread.Sleep(500);
            } else if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD5)) {
                Rapid_Fire_Checkbox.IsChecked = !Rapid_Fire_Checkbox.IsChecked;
                Thread.Sleep(500);
            } else if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD6)) {
                Infinite_Grenades_Checkbox.IsChecked = !Infinite_Grenades_Checkbox.IsChecked;
                Thread.Sleep(500);
            } else if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD7)) {
                Third_Person_Checkbox.IsChecked = !Third_Person_Checkbox.IsChecked;
                Thread.Sleep(500);
            } else if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD8)) {
                One_Hit_Kill_Checkbox.IsChecked = !One_Hit_Kill_Checkbox.IsChecked;
                Thread.Sleep(500);
            } else if (input.InputDeviceState.IsKeyDown(VirtualKeyCode.NUMPAD9)) {
                Low_Gravity_Checkbox.IsChecked = !Low_Gravity_Checkbox.IsChecked;
                Thread.Sleep(500);
            }
        }

        private void Set_Currency_Button_Click(object sender, RoutedEventArgs e) {
            var currencyName = Currency_Combo_Box.Text;
            var currencyValue = Currency_TextBox.Text;

            if (currencyName == "Cash") {
                if (Freeze_Currency_Checkbox.IsChecked == true) {
                    Memlib.FreezeValue("0x25E4BEE4E", "int", currencyValue);
                    Log_TextBox.Text = "Cash Frozen at " + currencyValue;
                } else {
                    Memlib.UnfreezeValue("0x25E4BEE4E");
                    Memlib.WriteMemory("0x25E4BEE4E", "int", currencyValue);
                    Log_TextBox.Text = "Cash set to " + currencyValue;
                }
            } else if (currencyName == "Tickets") {
                if (Freeze_Currency_Checkbox.IsChecked == true) {
                    Memlib.FreezeValue(addressBase + "5D16904", "int", currencyValue);
                    Memlib.FreezeValue(addressBase + "67B00D8", "int", currencyValue);
                    Memlib.FreezeValue(addressBase + "4295688", "int", currencyValue);
                    Memlib.FreezeValue(addressBase + "42920C8", "int", currencyValue);
                    Memlib.FreezeValue(addressBase + "51D7290", "int", currencyValue);
                    Memlib.FreezeValue(addressBase + "67B0438", "int", currencyValue);
                    Log_TextBox.Text = "Tickets Frozen at " + currencyValue;
                } else {
                    Memlib.UnfreezeValue(addressBase + "5D16904");
                    Memlib.UnfreezeValue(addressBase + "67B00D8");
                    Memlib.UnfreezeValue(addressBase + "4295688");
                    Memlib.UnfreezeValue(addressBase + "42920C8");
                    Memlib.UnfreezeValue(addressBase + "51D7290");
                    Memlib.UnfreezeValue(addressBase + "67B0438");
                    Memlib.WriteMemory(addressBase + "5D16904", "int", currencyValue);
                    Memlib.WriteMemory(addressBase + "67B00D8", "int", currencyValue);
                    Memlib.WriteMemory(addressBase + "4295688", "int", currencyValue);
                    Memlib.WriteMemory(addressBase + "42920C8", "int", currencyValue);
                    Memlib.WriteMemory(addressBase + "51D7290", "int", currencyValue);
                    Memlib.WriteMemory(addressBase + "67B0438", "int", currencyValue);
                    Log_TextBox.Text = "Tickets set to " + currencyValue;
                }
            }
        }
    }
}
