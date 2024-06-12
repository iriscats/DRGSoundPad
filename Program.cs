using DRGSoundPad;

Console.WriteLine("Hello, World!");


/*await TTSServer.DownloadFile("你好世界 we are rich", "a.wav");

var names = SpeakServer.GetOutputAudioDeviceNames();

foreach (var name in names)
    Console.WriteLine(name);

var id = SpeakServer.GetOutputDeviceID("CABLE Input (VB-Audio Virtual C");
//var id = SpeakServer.GetOutputDeviceID("耳机 (猫王·小王子OTR+ Stereo)");

Console.WriteLine(id);

SpeakServer.PlayAudioToSpecificDevice("a.wav", id, false, 100, false);
SpeakServer.PlayAudioex("a.wav", 0, 100);


MessageBox.Show("");*/


NamedPipeServer.Main();
