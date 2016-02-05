using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeechLib;
using System.Speech.Synthesis;
namespace ConsoleApplication5
{
    static class speak
    {
        private static SpeechSynthesizer speech = new SpeechSynthesizer();
        public static void say(String ch)
        {
            speech.Speak(ch);
        }
        public static void setRate(int rate)
        {
            speech.Rate = rate;
        }

        public static void setVolume(int volume)
        {
            speech.Volume = volume;
        }

        public static void setVoice(String name)
        {

            speech.SelectVoice(name);

        }

        public static String getVoices()
        {
            String a = "";
            foreach (InstalledVoice b in speech.GetInstalledVoices())
            {
                a += b.VoiceInfo.Name + "\0";
            }
            return a;
        }

        public static String getEtat() {
            String a = speech.Voice.Name;
            a += "\0" + speech.Volume.ToString();
            a += "\0" + speech.Rate.ToString();

            return a; 
        }
    }
}
