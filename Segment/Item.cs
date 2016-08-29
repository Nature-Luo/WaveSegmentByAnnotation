using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Segment
{
    class Item
    {
        private string _str_name;
        private int _int_size;
        private List<double> _db_x;
        private List<string> _text;

        public string Name {
            get { return _str_name;}
            set{_str_name=value;}
        }

        public int Size {
            get { return _int_size; }
            set { _int_size = value; }
        }

        public List<double> X
        {
            get { return _db_x; }
            set { _db_x = value; }
        }

        public List<string> Text {
            get { return _text; }
            set { _text = value; }
        }

        public Item() {
            this.Name = "";
            this.Size = 0;
            this.X = new List<double>();
            this.Text = new List<string>();

            // 保持号段一致，所以 x&text 从1开始
            X.Add(0);
            Text.Add(string.Empty);
        }
    }
}
