using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIPP.Web.Chem.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }


        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize, int totalcount)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalcount;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);




            this.AddRange(this.getContent(source,pageIndex,pageSize));

           // this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize).ToList());
        }

        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            this.AddRange(this.getContent(source, pageIndex, pageSize));
           // this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize).ToList());
        }

        public PaginatedList(IList<T> source, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            this.AddRange(this.getContent(source, pageIndex, pageSize));
            //this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
        }

        public PaginatedList(IList<T> source, int pageIndex, int pageSize, int totalcount)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalcount;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            this.AddRange(this.getContent(source, pageIndex, pageSize));
            //this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex + 1 < TotalPages);
            }
        }

        private IQueryable<T> getContent(IQueryable<T> source, int pageIndex, int pageSize)
        {
            return source.Take(pageSize * pageIndex).Skip(PageSize * (pageIndex - 1));
        }

        private IList<T> getContent(IList<T> source, int pageIndex, int pageSize)
        {
            return source.Take(pageSize * pageIndex).Skip(PageSize * (pageIndex - 1)).ToList();
        }
    }
}

