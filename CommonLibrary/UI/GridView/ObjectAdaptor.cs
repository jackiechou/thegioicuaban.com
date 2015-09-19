using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
/// <summary>
/// A generic adaptor for paged binding across any sort of object
/// </summary>

[DataObject]
public class ObjectAdaptor
{
    private readonly IList<object> _list;
    private readonly int _count;

    public ObjectAdaptor(IList<object> list, int count)
    {
        _list = list;
        _count = count;
    }

    public int Count()
    {
        return _count;
    }

    #region Select overloads used by the ObjectDataSource (all do exactly the same)
    [DataObjectMethod(DataObjectMethodType.Select, true)]
    public IList<object> Select()
    {
        return _list;
    }

    [DataObjectMethod(DataObjectMethodType.Select, false)]
    public IList<object> Select(int startRowIndex, int maximumRows)
    { //startRowIndex and maximumRows are the default names that asp uses
        return _list;
    }

    [DataObjectMethod(DataObjectMethodType.Select, false)]
    public IList<object> Select(string sortBy, int startRowIndex, int maximumRows)
    {
        return _list;
    }

    [DataObjectMethod(DataObjectMethodType.Select, false)]
    public IList<object> Select(string sortBy)
    {
        return _list;
    }
    #endregion
}

