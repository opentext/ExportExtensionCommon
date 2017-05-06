using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace ExportExtensionCommon
{
    #region TVIModel
    /// In order to implement a tree view in SIIE one needs to implement an instance of the
    /// TreeViewItemModel (TVIModel). A TVIModel must
    /// * Maintain the Name attribute to hold the string to be displayed at a tree node
    /// * Maintain all information needed to work with that node
    /// It is serialized so the information maintained may coexist in workable form and
    /// as a serialized form, e.g. DirectoryInfo + FullPathName. Note that the xml serializer
    /// is being used. Only public properties are serialized.
    /// 
    public abstract class TVIModel
    {
        // This is used to identify the node
        public string Id { get; set; }

        // The name as shown in the tree and identifying this node
        public string DisplayName { get; set; }

        // Depth in the tree, starts from 0
        public int Depth { get; set; }

        // Indicates whether this node can be expanded
        public virtual bool IsFolder { get; set; } = true;

        // The icon to be displayed for this node
        public virtual string Icon { get { return IsFolder ? "Folder" : "Default"; } }

        // Return all children for the current node
        public abstract List<TVIModel> GetChildren();

        // String use to concatenate two nodes in the final display string, e.g. "/" or "\" or "->"
        public virtual string GetPathConcatenationString() { return "/"; }

        // Name of node type, e.g. "File system folder"; is used for error message
        public abstract string GetTypeName();

        // Pathname is the Name of all nodes concatenated by the concatenation string
        public enum Pathtype { Id, DisplayName };
        public virtual string GetPath(List<TVIModel> path, Pathtype pt)
        {
            string result = string.Empty;
            foreach (TVIModel node in path)
                result += (pt == Pathtype.DisplayName? node.DisplayName : node.Id) + GetPathConcatenationString();
            return result;
        }

        // Compare two names. Overwrite if e.g. name is case insensitive
        public virtual bool IsSame(string id) { return Id == id; }

        public abstract TVIModel Clone();
    }
    #endregion

    #region SIEETreeView
    /// The SIEETreeView is the anchor of the tree.
    /// And it is also the type of the Children member.
    public class SIEETreeView : ObservableCollection<TVIViewModel>
    {
        public SIEEViewModel Vm { get; set; }

        public SIEETreeView(SIEEViewModel vm)
        {
            this.Vm = vm;
        }

        public void AddItem(TVIViewModel item)
        {
            this.Add(item);
            item.Vm = Vm;
            item.Children.Vm = Vm;
        }

        // Open the tree view according to the serialzed path.
        // Expands all nodes on the way
        public TVIViewModel InitializeTree(List<string> serializedPath, Type modelType)
        {
            if (serializedPath == null) return null;

            SIEETreeView currentTree = this;
            TVIViewModel currItem = null;
            try
            {
                for (int i = 0; i != serializedPath.Count; i++)
                {
                    TVIModel tvim = (TVIModel)TVIViewModel.deSerialize(serializedPath[i], modelType);

                    currItem = currentTree.findChild(tvim.Id);
                    if (currItem == null) throw new Exception(tvim.DisplayName + " not found");
                    currItem.IsExpanded = true;
                    currItem.IsSelected = true;
                    currentTree = currItem.Children;
                }
            }
            catch (Exception e)
            {
                string errMsg = "No items in tree view";
                string title = "Error";
                if (this.Count() > 0)
                {
                    TVIModel someModel = this[0].Tvim;
                    errMsg = "Could not locate " + someModel.GetTypeName() + ". Reason:\n" + e.Message;
                    title = "Navigate to " + someModel.GetTypeName();
                }
                SIEEMessageBox.Show(errMsg, title, System.Windows.MessageBoxImage.Error );
            }
            return currItem;
        }

        // Find the node in the tree starting at the current node
        // Expand tree if necessary
        public TVIViewModel FindNodeInTree(string idPath)
        {
            if (this.Count() == 0) return null;
            string concatString = this[0].Tvim.GetPathConcatenationString();
            string[] tokens = idPath.Split(new[] { concatString }, StringSplitOptions.None);

            TVIViewModel currNode = null;
            SIEETreeView currTree = this;
            for (int i = 0; i != tokens.Length; i++)
            {
                if (tokens[i] == string.Empty) continue;
                currNode = currTree.findChild(tokens[i]);
                if (currNode == null) return null;
                currNode.IsExpanded = true;
                currTree = currNode.Children;
            }
            return currNode;
        }

        // Find direct child
        private TVIViewModel findChild(string childId)
        {
            foreach (TVIViewModel child in this)
                if (child.Tvim.IsSame(childId)) return child;
            return null;
        }
    }
    #endregion

    #region Tree view: Item view model
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    public class TVIViewModel : ModelBase
    {
        #region Construction
        static readonly TVIViewModel DummyChild = new TVIViewModel();

        // This list can be extended when using the tree
        static Dictionary<string, string> IconMap = new Dictionary<string, string>() {
            { "Folder", "pack://application:,,,/ExportExtensionCommon.Base;component/Resources/folder.png" },
            { "Default", "pack://application:,,,/ExportExtensionCommon.Base;component/Resources/document.png" },
        };

        public TVIViewModel(TVIModel model, TVIViewModel parent, bool lazyLoadChildren)
        {
            Tvim = model;
            this.parent = parent;
            Tvim.Depth = parent == null ? 0 : parent.Tvim.Depth + 1;
  
            children = new SIEETreeView(null);
            if (lazyLoadChildren && Tvim.IsFolder)
                children.Add(DummyChild);
        }

        // This is used to create the DummyChild instance.
        private TVIViewModel() { }

        //public void SetAsParent() { parent = null; }
        #endregion

        #region Properties
        private TVIModel tvim;
        public TVIModel Tvim
        {
            get { return tvim; }
            set { SetField(ref tvim, value); RaisePropertyChanged("DisplayName"); }
        }

        public SIEEViewModel Vm { get; set; } = null;

        public string DisplayName { get { return Tvim.DisplayName; } }

        /// Returns the logical child items of this object.
        readonly SIEETreeView children;
        public SIEETreeView Children
        {
            get { return children; }
        }

        /// Returns true if this object's Children have not yet been populated.
        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }

        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                SetField(ref isExpanded, value);

                // Expand all the way up to the root.
                if (isExpanded && parent != null)
                    parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }
            }
        }

        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetField(ref isSelected, value); }
        }

        private TVIViewModel parent;
        public TVIViewModel Parent
        {
            get { return parent; }
        }

        public string Icon { get {
            return IconMap[Tvim != null && IconMap.ContainsKey(Tvim.Icon)? Tvim.Icon : "Folder"];
        } }
        #endregion

        #region Functions
        protected void LoadChildren()
        {
            if (Vm != null) Vm.IsRunning = true;
            try
            {
                foreach (TVIModel child in Tvim.GetChildren())
                    Children.AddItem(new TVIViewModel(child.Clone(), this, true));
            }
            finally { if (Vm != null) Vm.IsRunning = false; }
        }

        // Recursive function to create forward path to current node
        public List<TVIModel> GetPath()
        {
            List<TVIModel> result;
            if (Parent == null)
            {
                result = new List<TVIModel>();
                result.Add(Tvim);
                return result;
            }
            result = Parent.GetPath();
            result.Add(Tvim);
            return result;
        }

        //public string GetIdPath()
        //{
        //    return Tvim.GetPath(GetPath(), TVIModel.Pathtype.Id);
        //}

        public string GetDisplayNamePath()
        {
            return Tvim.GetPath(GetPath(), TVIModel.Pathtype.DisplayName);
        }

        public List<string> GetSerializedPath()
        {
            List<string> result;
            if (Parent == null)
            {
                result = new List<string>();
                result.Add(serialize(Tvim));
                return result;
            }
            result = Parent.GetSerializedPath();
            result.Add(serialize(Tvim));
            return result;
        }

        public static TVIModel GetSelectedItem(List<string> serializedFolderPath, Type modelType)
        {
            if (serializedFolderPath.Count == 0) return null;
            return deSerialize(serializedFolderPath.Last(), modelType) as TVIModel;
        }

        public static string serialize(object o)
        {
            return Serializer.SerializeToXmlString(o, System.Text.Encoding.Unicode);
        }
        public static object deSerialize(string s, Type t)
        {
            return Serializer.DeserializeFromXmlString(s, t, System.Text.Encoding.Unicode);
        }
       #endregion
    }
    #endregion
}