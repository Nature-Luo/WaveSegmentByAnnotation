using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace Segment
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
                Console.WriteLine("Usage: exe InputFolder OutputFolder SoxPath TextGrid|TextCheck");
            else if (Directory.Exists(args[0]))
            {
                if (!Directory.Exists(args[1]))
                    Directory.CreateDirectory(args[1]);

                //RunSegment(args[0], args[1], args[2]);

                List<string> list_subds=new List<string>();
                GetAllFolders(args[0], list_subds);
                string inf_orig = Regex.Replace(args[0], @"\\", "#");
                string outf_orig = Regex.Replace(args[1], @"\\", "#");
                
                foreach (string subd in list_subds) {

                    string outf = Regex.Replace(Regex.Replace(subd, @"\\", "#"), inf_orig, outf_orig);
                    outf = Regex.Replace(outf,"#", @"\" );
                    RunSegment(subd, outf, args[2],args[3]);
                }
            }
        }

        private static void RunSegment(string inf, string outf, string sox, string ttype)
        {
            string[] files = Directory.GetFiles(inf, "*."+ttype);

            foreach (string file in files) {
                string fn = Path.GetFileNameWithoutExtension(file);
                
                Trans_Seg(file,Path.Combine(outf,fn),sox,ttype);
            }

        }

        private static void Trans_Seg(string file, string outf, string sox, string ttype)
        {
            TextGrid tg = new TextGrid(file);
            //string wavef = Regex.Replace(file, "."+ttype, ".wav",RegexOptions.IgnoreCase);
            string wavef = Path.GetDirectoryName(file) +@"\"+ Path.GetFileNameWithoutExtension(file) + ".wav";

            if (!File.Exists(wavef))
            {
                Console.WriteLine("!!!Error: cannot find "+wavef);
                Environment.Exit(1);
            }
            string fn = Path.GetFileNameWithoutExtension(file);

            if (!Directory.Exists(outf))
                Directory.CreateDirectory(outf);

            foreach (var item in tg.List_Items) {
                if (item.Name == "CONTENT") {
                    double db_start = 0;
                    double db_leng = 0;
                    for (int i = 0; i < item.Text.Count; i++)
                    {
                        if ((item.Text[i] != "") && (item.Text[i] != "+") && (!Regex.IsMatch(item.Text[i],@"^\[\w+\]$")))
                        {
                            db_start = item.X[i];
                            if (i < item.X.Count - 1)
                                db_leng = item.X[i + 1] - item.X[i];
                            else
                                db_leng = tg.Length - item.X[i];
                            System.Diagnostics.Process p = new System.Diagnostics.Process();
                            p.StartInfo.FileName = sox;
                            p.StartInfo.Arguments = wavef + " " + outf + @"\" + fn + "#" + i.ToString() + ".wav" + " trim " + db_start + " " + db_leng;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.CreateNoWindow = true;
                            p.Start();

                            //if (!File.Exists(outf + @"\" + fn + "#" + i.ToString() + ".wav"))
                            //{
                            //    Console.WriteLine("failed to segement file " + outf + @"\" + fn + "#" + i.ToString() + ".wav");
                            //    Environment.Exit(0);
                            //}

                            if (p.HasExited)
                                p.Kill();
                            
                            using (StreamWriter sw = new StreamWriter(outf + @"\" + fn + "#" + i.ToString() + ".txt"))
                            {
                                sw.WriteLine(item.Text[i]);
                            }

                        }
                    }

                }
            }
        }

        private static void GetAllFolders(string InputFolder, List<string> list_dn)
        {
            list_dn.Add(InputFolder);
            string[] subds=Directory.GetDirectories(InputFolder);
            if (subds.Length >= 1) {
                for (int i = 0; i < subds.Length; i++)
                    GetAllFolders(subds[i], list_dn);
            }
        }
    }
}
