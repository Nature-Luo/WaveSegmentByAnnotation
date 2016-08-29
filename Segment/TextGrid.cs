using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Segment
{
    class TextGrid
    {
        private double _db_length;
        private int _int_size;
        private List<Item> _list_items;

        public double Length{
            get { return _db_length; }
            set { _db_length = value; }
        }
        public int Size {
            get { return _int_size; }
            set { _int_size = value; }
        }
        public List<Item> List_Items {
            get { return _list_items; }
            set { _list_items = value; }
        }

        public void Load(string file_TG) {

            List<Item> items = new List<Item>();
            
            //string[] lines = File.ReadAllLines(file_TG, GetFileEncodeType(file_TG));
            using(StreamReader sr=new StreamReader(file_TG,GetFileEncodeType(file_TG))){
                string line=sr.ReadLine();
                while(line!=null){

            //foreach (string line in lines) {
                if (Regex.IsMatch(line, "^xmax = (.*?) $")) {
                    Match mat = Regex.Match(line, "^xmax = (.*?) $");
                    this.Length = Convert.ToDouble(mat.Groups[1].ToString());
                }
                else if (Regex.IsMatch(line, "^size = (.*?) $")) {
                    Match mat = Regex.Match(line, "^size = (.*?) $");
                    this.Size = Convert.ToInt16(mat.Groups[1].ToString());
                }
                else if (Regex.IsMatch(line, @"^    item \[\d\]:$")) {
                    items.Add(new Item());
                }
                else if(Regex.IsMatch(line, "name = \"(.*?)\" $")){
                    Match mat = Regex.Match(line, "name = \"(.*?)\" $");
                    items[items.Count - 1].Name = mat.Groups[1].ToString();
                }
                else if (Regex.IsMatch(line, "intervals: size = (.*?) $"))
                {
                    Match mat = Regex.Match(line, "intervals: size = (.*?) $");
                    items[items.Count - 1].Size = Convert.ToInt16(mat.Groups[1].ToString());
                }
                else if (Regex.IsMatch(line, @"        intervals \[\d+\]:$"))
                {
                    string tmp = sr.ReadLine();
                    //Match mat = Regex.Match(tmp, "xmin = (.*?) $");
                    Match mat = Regex.Match(tmp, "xmin = (.*?)$");
                    items[items.Count - 1].X.Add(Convert.ToDouble(mat.Groups[1].ToString()));

                    tmp = sr.ReadLine();
                    tmp = sr.ReadLine();
                    mat = Regex.Match(tmp, @"text = ""(.*?)"" $");
                    items[items.Count - 1].Text.Add(mat.Groups[1].ToString());

                }

                line = sr.ReadLine();
            }

            this.List_Items = items;
        }
        }

        /************ get TextGrid Encoding ****************/
        public System.Text.Encoding GetFileEncodeType(string filename)
        {
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            Byte[] buffer = br.ReadBytes(2);
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    return System.Text.Encoding.UTF8;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return System.Text.Encoding.Unicode;
                }
                else
                {
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                return System.Text.Encoding.Default;
            }
        }

        public TextGrid(string TGFile) {
            Load(TGFile);
        }
    }

}
