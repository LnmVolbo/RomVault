/******************************************************
 *     ROMVault2 is written by Gordon J.              *
 *     Contact gordon@romvault.com                    *
 *     Copyright 2014                                 *
 ******************************************************/
 
using System;
using System.IO;
using ROMVault2.RvDB;
 
namespace ROMVault2
{
    public static class DatMaker
    {
        private static StreamWriter _sw;
        private static string _datName;
        private static string _datDir;
        public static void MakeDatFromDir(RvDir startingDir)
        {
            _datName = startingDir.Name;
            _datDir = startingDir.Name;
            Console.WriteLine("Creating Dat: " + startingDir.Name + ".dat");
            _sw = new StreamWriter(startingDir.Name + ".dat");
 
            WriteDatFile(startingDir);
 
            _sw.Close();
 
            Console.WriteLine("Dat creation complete");
            Console.ReadLine();
        }
 
        private static void WriteDatFile(RvDir dir)
        {
            WriteLine("<?xml version=\"1.0\"?>");
            WriteLine("");
            WriteLine("<datafile>");
            WriteHeader();
 
            /* write Games/Dirs */
            ProcessDir(dir);
 
            WriteLine("</datafile>");
        }
 
        private static void WriteHeader()
        {
            WriteLine("    <header>");
            WriteLine("        <name>" + clean(_datName) + "</name>");
            WriteLine("        <rootdir>" + clean(_datDir) + "</rootdir>");
            WriteLine("    </header>");
        }
 
        private static void WriteLine(string s)
        {
            _sw.WriteLine(s);
        }
 
        private static string clean(string s)
        {
            s = s.Replace("\"", "&quot;");
            s = s.Replace("'", "&apos;");
            s = s.Replace("<", "&lt;");
            s = s.Replace(">", "&gt;");
            s = s.Replace("&", "&amp;");
            return s;
        }
 
        // cuts off zip extension only when present, avoiding damage when filenames containing periods have already had their extension removed.
        private static string GetFileNameWithoutZipExtension(string fileName)
        {
            if (fileName.EndsWith(".zip", System.StringComparison.OrdinalIgnoreCase))
            {
                return fileName.Substring(0, fileName.Length - 4);
            }
            return fileName;
        }
 
        private static void ProcessDir(RvDir dir, int depth = 1)
        {
            string d = new string(' ', 4 * depth);
 
            for (int i = 0; i < dir.ChildCount; i++)
            {
                RvDir game = dir.Child(i) as RvDir;
                if (game != null && game.FileType == FileType.Zip)
                {
// this original code nukes game names with periods in the name, 
// since Path.GetFileNameWithoutExtension() is blind as to whether the extension has already been stripped.
//                  WriteLine(d + "<game name=\"" + clean(Path.GetFileNameWithoutExtension(game.Name)) + "\">");
//                  WriteLine(d + "    <description>" + clean(Path.GetFileNameWithoutExtension(game.Name)) + "</description>");
 
// this code removes only the period and zip extension when present.
//                  string gameNameDesc = clean(GetFileNameWithoutZipExtension(game.Name));
// this code assumes that the game.Name already has the extension removed.              
                    string gameNameDesc = clean(game.Name);
 
                    WriteLine(d + "<game name=\"" + gameNameDesc + "\">");
                    WriteLine(d + "    <description>" + gameNameDesc + "</description>");
// end of modifications                    
                    for (int j = 0; j < game.ChildCount; j++)
                    {
                        RvFile file = game.Child(j) as RvFile;
                        if (file != null)
                        {
                            WriteLine(d + "    <rom name=\"" + clean(file.Name) + "\" size=\"" + file.Size + "\" crc=\"" + Utils.ArrByte.ToString(file.CRC) + "\" md5=\"" + Utils.ArrByte.ToString(file.MD5) + "\" sha1=\"" + Utils.ArrByte.ToString(file.SHA1) + "\"/>");
                        }
                    }
                    WriteLine(d + "</game>");
                }
                if (game != null && game.FileType == FileType.Dir)
                {
                    WriteLine(d + "<dir name=\"" + clean(game.Name) + "\">");
                    ProcessDir(game, depth + 1);
                    WriteLine(d + "</dir>");                    
                }
            }
        }
    }
}
