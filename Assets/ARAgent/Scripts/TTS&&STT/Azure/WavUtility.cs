using UnityEngine;
using System.IO;

public static class WavUtility
{
    /// <summary>
    /// Converts an AudioClip to a byte array containing a WAV file.
    /// </summary>
    public static byte[] FromAudioClip(AudioClip clip)
    {
        // Create a new WAV file
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        // Write the WAV header
        writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
        writer.Write(36 + clip.samples * 2);
        writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
        writer.Write(new char[4] { 'f', 'm', 't', ' ' });
        writer.Write(16);
        writer.Write((ushort)1);
        writer.Write((ushort)clip.channels);
        writer.Write(clip.frequency);
        writer.Write(clip.frequency * clip.channels * 2);
        writer.Write((ushort)(clip.channels * 2));
        writer.Write((ushort)16);
        writer.Write(new char[4] { 'd', 'a', 't', 'a' });
        writer.Write(clip.samples * 2);

        // Write the audio data
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);
        int intMax = 32767; // max value for a 16-bit signed integer
        for (int i = 0; i < clip.samples; i++)
        {
            writer.Write((short)(samples[i] * intMax));
        }

        // Clean up
        writer.Close();
        byte[] wavBytes = stream.ToArray();
        stream.Close();
        return wavBytes;
    }
}
