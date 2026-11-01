# Sanitized snapshot for reading. Machine-specific and not runnable as is. Generated from tools/ui.ps1.
# Thin desktop control: real keyboard/mouse through SendInput and screen capture. Dot-source it, then call the helpers.
Add-Type -AssemblyName System.Drawing, System.Windows.Forms
if (-not ('Ui' -as [type])) {
Add-Type @'
using System; using System.Runtime.InteropServices; using System.Threading;
public static class Ui {
  [StructLayout(LayoutKind.Sequential)] struct MI { public int dx, dy; public uint data, flags, time; public IntPtr extra; }
  [StructLayout(LayoutKind.Sequential)] struct KI { public ushort vk, scan; public uint flags, time; public IntPtr extra; public long pad; }
  [StructLayout(LayoutKind.Explicit)] struct IN { [FieldOffset(0)] public uint type; [FieldOffset(8)] public MI m; [FieldOffset(8)] public KI k; }
  [DllImport("user32")] static extern uint SendInput(uint n, IN[] i, int size);
  [DllImport("user32")] public static extern IntPtr FindWindow(string c, string t);
  [DllImport("user32")] public static extern bool SetForegroundWindow(IntPtr h);
  [DllImport("user32")] public static extern IntPtr GetForegroundWindow();
  [DllImport("user32")] public static extern bool ShowWindow(IntPtr h, int c);
  [DllImport("user32")] public static extern bool SetCursorPos(int x, int y);
  [DllImport("user32")] static extern void keybd_event(byte v, byte s, uint f, UIntPtr e);
  [DllImport("user32")] public static extern int GetSystemMetrics(int i);
  public static void Focus(IntPtr h) { keybd_event(0x12,0,0,UIntPtr.Zero); keybd_event(0x12,0,2,UIntPtr.Zero); ShowWindow(h,9); SetForegroundWindow(h); }
  static void Send(IN i) { SendInput(1, new[]{i}, Marshal.SizeOf(typeof(IN))); }
  public static void Scan(ushort scan, bool up) { var i=new IN{type=1}; i.k.scan=scan; i.k.flags=8u|(up?2u:0u); Send(i); }
  public static void Press(ushort scan, int ms) { Scan(scan,false); Thread.Sleep(ms); Scan(scan,true); }
  public static void Unicode(char c) { var i=new IN{type=1}; i.k.scan=c; i.k.flags=4; Send(i); i.k.flags=6; Send(i); }
  public static void Move(int x,int y) { var i=new IN{type=0}; i.m.dx=x*65535/(GetSystemMetrics(0)-1); i.m.dy=y*65535/(GetSystemMetrics(1)-1); i.m.flags=0x8001; Send(i); }
  public static void Rel(int x,int y) { var i=new IN{type=0}; i.m.dx=x; i.m.dy=y; i.m.flags=1; Send(i); }
  public static void Button(uint down,uint up,int ms) { var i=new IN{type=0}; i.m.flags=down; Send(i); Thread.Sleep(ms); i.m.flags=up; Send(i); }
}
'@
}
$script:Scan = @{ e=0x12; esc=0x01; tab=0x0F; enter=0x1C; space=0x39; w=0x11; a=0x1E; s=0x1F; d=0x20; f5=0x3F; lalt=0x38; lshift=0x2A; lctrl=0x1D; b=0x30; q=0x10; r=0x13; c=0x2E; f=0x21; '1'=0x02; '2'=0x03; '3'=0x04 }
function Game { (Get-Process valheim -ErrorAction SilentlyContinue | Where-Object MainWindowHandle -ne 0 | Select-Object -First 1) }
function FocusGame { $p = Game; if ($p) { [Ui]::Focus($p.MainWindowHandle); Start-Sleep -Milliseconds 300 } }
function Shot($path, $x = 0, $y = 0, $w = 2560, $h = 1440) {
  $b = New-Object Drawing.Bitmap $w, $h; $g = [Drawing.Graphics]::FromImage($b)
  $g.CopyFromScreen($x, $y, 0, 0, (New-Object Drawing.Size $w, $h)); $g.Dispose()
  $s = New-Object Drawing.Bitmap $b, ([int]($w * 0.5)), ([int]($h * 0.5)); $s.Save($path, [Drawing.Imaging.ImageFormat]::Png); $s.Dispose(); $b.Dispose()
}
function Key($k, $ms = 80) { [Ui]::Press([ushort]$script:Scan[$k], $ms) }
function Click($x, $y, $ms = 80) { [Ui]::Move($x, $y); Start-Sleep -Milliseconds 120; [Ui]::Button(2, 4, $ms) }
function RightClick($x, $y) { [Ui]::Move($x, $y); Start-Sleep -Milliseconds 120; [Ui]::Button(8, 16, 80) }
function TypeText($text) { foreach ($c in $text.ToCharArray()) { [Ui]::Unicode($c); Start-Sleep -Milliseconds 30 } }
