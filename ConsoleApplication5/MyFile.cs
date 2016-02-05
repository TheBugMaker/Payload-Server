using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5
{
    class MyFile
    {
        private String id  ;
        private System.IO.FileStream fs;

        public MyFile(String id, System.IO.FileStream fs)
        {
            this.id = id;
            this.fs = fs; 
        }
        public bool close(String id) {
            if (this.id.Equals(id)) { fs.Close(); return true; }
            return false; 
        }
        public System.IO.FileStream getHandle() {
            return fs; 
        }

        public bool hasId(String id){
            return id == this.id; 
        }
    }
}
