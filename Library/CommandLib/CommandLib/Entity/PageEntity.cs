using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommandLib.Entity
{
    public class PageEntity
    {
        public PageEntity() { }
        public PageEntity(int dataCount, int pageSize, int linkPageNum = 1, int currentPage = 1) 
        {
            DataCount = dataCount;
            PageSize = pageSize;
            PageCount = dataCount % pageSize != 0 ? (dataCount / pageSize) + 1 : (dataCount / pageSize);
            if (linkPageNum < 1) { linkPageNum = 1; }
            LinkPageNum = linkPageNum > PageCount ? PageCount : linkPageNum;
            CurrentPage = currentPage;
        }
        public int DataCount { set; get; }
        public int PageCount { set; get; }
        public int PageSize { set; get; }
        public int CurrentPage { set; get; }
        public int LinkPageNum { set; get; }
        public IEnumerable DataSource { set; get; }
    }
}
