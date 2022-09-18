using Eloi;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConvertVideoAtPathToUnityPlayer : MonoBehaviour
{
    public string[] m_fileToConvert;
    public string m_resultFolder;
    public string m_ffmpegPath;

    public int m_width= 4096;
    public int m_height= 4096;


    public void SetResolutionDropBox(int value) {

        switch (value)
        {
            case 0: Set4KResolution(); break;
            case 1: Set5KResolution(); break;
            case 2: Set5K21RatioResolution(); break;
            default:
                break;
        }
    }

    [ContextMenu("Set4KResolution")]
    public void Set4KResolution()
    {
        m_width = 4096;
        m_height = 4096;
    }
    [ContextMenu("Set5KResolution")]
    public void Set5KResolution()
    {
        m_width = 5760;
        m_height = 5760;
    }
    [ContextMenu(" Set5K21RatioResolution")]
    public void Set5K21RatioResolution()
    {
        m_width = 5120;
        m_height = 2560;
    }


    [TextArea(0, 10)]
    public string m_ffmpegCommand4K = "FFMPEG -y -i \"INPUT\" -c:v libx264 -preset fast -crf 18 -x264-params mvrange=511 -maxrate 50M -bufsize 25M -vf \"scale=WIDTHxHEIGHT\" - pix_fmt yuv420p -c:a aac -b:a 160k -movflags faststart \"OUTPUT\"";
    public bool m_deleteBatAfter;
    public string m_lastSendCommand;
    public Eloi.MetaFileNameWithExtension fileToConvert;
    public Eloi.MetaFileNameWithExtension fileConverted;
    public Eloi.MetaFileNameWithoutExtension fileConvertedNoExt;
    public MetaAbsolutePathDirectory whereToStore;


    public void SetFfmpegPath(string pathOfExecutable)
    {
        m_ffmpegPath = pathOfExecutable;
    }
    public void SetWhereToStore(string storagePath)
    {
        m_resultFolder = storagePath;
    }
    public void SetPathsToConvert(string[] paths)
    {
        m_fileToConvert = paths;
    }
    public void SetPathsToConvert(string path)
    {
        m_fileToConvert = new string[] { path  };
    }


    [ContextMenu("Convert Test")]
    public void Convert()
    {
        foreach (string item in m_fileToConvert)
        {

            if (File.Exists(item))
            {
                ConvertPath(item);
            }
            else if (Directory.Exists(item)) {
                string [] paths = Directory.GetFiles(item,"*", SearchOption.AllDirectories);
                foreach (var pitem in paths)
                {
                    ConvertPath(pitem);
                }
            }
        }
    }

    private void ConvertPath(string item)
    {
        fileToConvert = new MetaFileNameWithExtension(Path.GetFileName(item));
        fileToConvert.GetFileNameWithoutExtension(out string name);
        fileConverted = new MetaFileNameWithExtension(name + "_C4k", "mp4");
        fileToConvert.GetFileNameWithoutExtension(out string nameC);
        fileConvertedNoExt = new MetaFileNameWithoutExtension(nameC);
        whereToStore = new MetaAbsolutePathDirectory(m_resultFolder.Length == 0 ? Path.GetDirectoryName(item) : m_resultFolder);
        IMetaAbsolutePathFileGet outPath = Eloi.E_FileAndFolderUtility.Combine(whereToStore, fileConverted);

        m_lastSendCommand = m_ffmpegCommand4K;
        m_lastSendCommand = m_lastSendCommand.Replace("INPUT", item);
        m_lastSendCommand = m_lastSendCommand.Replace("OUTPUT", outPath.GetPath());
        m_lastSendCommand = m_lastSendCommand.Replace("WIDTH", ""+m_width);
        m_lastSendCommand = m_lastSendCommand.Replace("HEIGHT", "" +m_height);
        if (!string.IsNullOrEmpty(m_ffmpegPath))
            m_lastSendCommand = m_lastSendCommand.Replace("FFMPEG", m_ffmpegPath);
        else
            m_lastSendCommand = m_lastSendCommand.Replace("FFMPEG", "ffmpeg");

        E_LaunchWindowBat.CreateAndExecuteBatFile(whereToStore, fileConvertedNoExt, in m_deleteBatAfter, m_lastSendCommand);
    }
}
