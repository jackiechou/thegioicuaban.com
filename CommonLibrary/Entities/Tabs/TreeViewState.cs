using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;


public class TreeViewState
{
	public TreeViewState(){}

    public void SaveTreeView(TreeView treeView, string key)
    {

        List<bool?> list = new List<bool?>();
        SaveTreeViewExpandedState(treeView.Nodes, list);
        HttpContext.Current.Session[key + treeView.ID] = list;
    }

    private int RestoreTreeViewIndex;

    public void RestoreTreeView(TreeView treeView, string key)
    {

        RestoreTreeViewIndex = 0;

        RestoreTreeViewExpandedState(treeView.Nodes,

            (List<bool?>)HttpContext.Current.Session[key + treeView.ID] ?? new List<bool?>());

    }
    
    private void SaveTreeViewExpandedState(TreeNodeCollection nodes, List<bool?> list)
    {
        foreach (TreeNode node in nodes)
        {
            list.Add(node.Expanded);

            if (node.ChildNodes.Count > 0)
            {
                SaveTreeViewExpandedState(node.ChildNodes, list);
            }

        }

    }
    
    private void RestoreTreeViewExpandedState(TreeNodeCollection nodes, List<bool?> list)
    {
        foreach (TreeNode node in nodes)
        {
            if (RestoreTreeViewIndex >= list.Count) break;

            node.Expanded = list[RestoreTreeViewIndex++];

            if (node.ChildNodes.Count > 0)
            {
                RestoreTreeViewExpandedState(node.ChildNodes, list);
            }
        }

    }
}