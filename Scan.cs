using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//This is the program that I did during the Data summarisation algorithms comparison project. 
//It use the DFS to travel the directry and create the tree structure. 
//Considering the pirvate issue, the output needs to be reformated which is the major probelm during the programming.
namespace DirectoryScan 
{
    class Scan
    {
        static string selectedDir = " ";
        static char pathDelimiter = '/';
        static void Main(string[] args)
        {
            // Take the 1st command as the directory name

            selectedDir = (args.Length > 0) ? args[0] : Directory.GetCurrentDirectory();
            TreeNode root = ScanDirectory.WalkDierctory(@selectedDir, null);
           
            // Take the 2nd command as the pathdelimiter

            Console.WriteLine("Start setting dictionary");
            pathDelimiter = (args.Length > 1) ? Convert.ToChar(args[1]) : '/';
            ScanDirectory.setDictionary(root, pathDelimiter);
            Console.WriteLine("Start output to txt file");
            ScanDirectory.CreateTreeTable();
            ScanDirectory.Output(root);
            ScanDirectory.Close();
         }
    }
    public class ScanDirectory
  {     
        static StreamWriter output; // Output of scan result

        // Dictionary use fullname of the file as a key to find the new pathname

        static Dictionary<string, string> findPath = new Dictionary<string, string>();

        // Dictionary use fullname of the file as a key to get the nodeID in the directory tree

        static Dictionary<string, string> filename = new Dictionary<string, string>(); 

       
      public static void CreateTreeTable()
      {
          // Initialise the output table with 7 columns

              output = new StreamWriter("output.txt");
              output.WriteLine("FilePath\tDirectoryPath\tFileName\tExtension\tSize\tCreated\tModified");
      }

      public static TreeNode WalkDierctory(string directory,TreeNode dupParent)
      {
          // Convert the directory name from string to directoryInfo  

            DirectoryInfo dir = new DirectoryInfo (directory);
            return  WalkDirectory(dir, dupParent);
      }

      
      public static TreeNode WalkDirectory(DirectoryInfo directory, TreeNode dupParent)
      {     
           // Create the root

           TreeNode dup = new TreeNode(dupParent, directory.Name);
           dup.Fullname = directory.FullName;
           dup.Extension = directory.Extension;
           dup.Modified = textDate(directory.LastWriteTime);
           dup.Created = textDate(directory.CreationTime);
           dup.Size = "null";
           dup.DirectoryName = directory.FullName;
            
          try
            {
                foreach (FileInfo file in directory.GetFiles())
                {   
                    // Ignore the hidden file

                    if ( !file.Name.StartsWith("."))
                    {
                     Console.WriteLine(file.FullName);

                    // Create the children node
                    TreeNode child = new TreeNode(dup, file.Name);
                    child.Fullname = file.FullName;
                    child.DirectoryName = file.DirectoryName;
                    child.Name = file.Name;
                    child.Extension = file.Extension.TrimStart('.');
                    child.Size = file.Length.ToString();
                    child.Created = textDate(file.CreationTime);
                    child.Modified = textDate(file.LastWriteTime);
                    }
                }
                
                // Find the maximum node number

                if (dup.Children.Count > TreeNode.MaxID)
                {
                    TreeNode.MaxID = dup.Children.Count; 
                }

                // Recursive for each subdirectory

                DirectoryInfo[] subDirectories = directory.GetDirectories();
                foreach (DirectoryInfo subDirectory in subDirectories)
                {  
                    // Ignore the hidden directory

                  if (!subDirectory .Name.StartsWith("."))
                   WalkDirectory(subDirectory,dup);
                }
            }

            catch (UnauthorizedAccessException e )
            {
                Console.WriteLine("Access denied: " + e.ToString()); 
            }
            return dup;
        }

        private static string textDate(DateTime time)
        {
            return time.ToString("yyyy/MM::MMM/dd");
        }
      
        public static void setDictionary(TreeNode node, char pathdelimiter)
        {
            // Set the key and value for 2 dictionaries 

            filename.Add(node.Fullname, node.NodeID()); 
            findPath.Add(node.Fullname, node.NamePath(pathdelimiter));
           
            // Recursive 
            foreach (TreeNode n in node.Children)
            {
                setDictionary(n, pathdelimiter);
            }
        }

        public static void Output(TreeNode node)
        {
            // Output the scan result
            foreach (TreeNode n in node.Children)
            {   
                // Ignore the node if it is directory 
                if (n.Size != "null")
                {   
                    // Use dictionary to get the new output
                    string pathname = findPath[n.Fullname];
                    string FileName = filename[n.Fullname];
                    string directoryName = findPath[n.DirectoryName];
                    output.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", pathname, directoryName, FileName, n.Extension, n.Size, n.Created, n.Modified);
                }
            }
            
            // Recursively process each sub node
 
            foreach (TreeNode subnode in node.Children)
            {
                Output(subnode);
            }
          }
        public static void Close()
        {
            output.Close(); 
        }
     }

    public class TreeNode
    {
        private TreeNode parent;         // Parent node, null when it is the root
        private List<TreeNode> children; // List of children 
        private int id;                  // A unique id under the parent      
        private string name;             // Name of the node
        private string directoryName;    // Directory name of the node
        private string extension;        // Extension of the node  
        private string size;             // Size of the node. If it is directory, size will be "null". 
        private string created;          // Created time of the node
        private string modified;         // Last modified time
        private string fullname;         // Full name of a node

        private static int maxID;       // Max number of nodes in a tree

        public TreeNode Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        public List<TreeNode> Children
        {
            get { return this.children; }
            set { this.children = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string DirectoryName
        {
            get { return this.directoryName; }
            set { this.directoryName = value; }
        }

        public string Extension
        {
            get { return this.extension; }
            set { this.extension = value; }
        }

        public string Size
        {
            get { return this.size; }
            set { this.size = value; }
        }

        public string Created
        {
            get { return this.created; }
            set { this.created = value; }
        }

        public string Modified
        {
            get { return this.modified; }
            set { this.modified = value; }
        }

        public string Fullname
        {
            get { return this.fullname; }
            set { this.fullname = value; }
        }

        public static int MaxID
        {
            get { return maxID; }
            set { maxID = value; }
        }

        public TreeNode(TreeNode parent, string fullname)
        {
            this.parent = parent;
            this.fullname = fullname;
            this.children = new List<TreeNode>();

            // If parent is not null add the node to the tree
            // If parent is null set the node id =1 (start point)

            if (parent != null)     
            {
                parent.children.Add(this);
                this.id = this.parent.children.Count;
            }
            else  
            {
                this.id = 1;
            }
        }

        public override string ToString()
        {
            string print = this.id + " " + this.name;
            return print;
        }

        public void PrintTree()
        {
            Console.WriteLine(this.ToString());
            foreach (TreeNode n in this.children)
                n.PrintTree();
        }

        public TreeNode Duplicate()
        {
            TreeNode copy = new TreeNode(null, this.name);
            return copy;
        }

         public static TreeNode DupTree(TreeNode original, TreeNode dupParent)
        {
            TreeNode dup = original.Duplicate();

            // Hook dup to parent

            dup.parent = dupParent;
            dup.parent.children.Add(dup);

            // Recursively process each sub node

            foreach (TreeNode c in original.children)
            {
                DupTree(c, dup);
            }
            return dup;
        }

        public string NodeID()
        {
            //Use the maxID to format the nodeID

            string output = null;

            int length = maxID.ToString().Length;
            output = "N" + this.id.ToString().PadLeft(length, '0');

            return output;
        }

        public List<TreeNode> NodePath()
        {
            //Create the nodepath for each node

            TreeNode cursor = this;
            List<TreeNode> path = new List<TreeNode>();
            path.Add(cursor);

            while (cursor.parent != null)
            {
                path.Add(cursor.parent);
                cursor = cursor.parent;
            }
            return path;
        }

        public string NamePath(char pathdelimiter)
        {
            // Create the fullpath of each node

            string namepath = null;

            for (int i = this.NodePath().Count - 1; i >= 0; i--)
            {
                namepath = namepath + pathdelimiter + this.NodePath()[i].NodeID();
            }
            return namepath;
        }
    }
}
