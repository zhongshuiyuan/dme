using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Common.Page
{
    public class SimplePage<T> : IPage
    {
        /**
        * 默认的分页大小
        */
        protected static int DEF_COUNT = 20;

        public Boolean LastPage { get; set; } = false;
        public Boolean FirstPage { get; set; } = false;
        /**
         * 分页数据总记录数
         */
        public int TotalCount { get; set; }
        /**
         * 分页大小
         */
        public int PageSize { get; set; } = DEF_COUNT;
        /**
         * 当前分页号
         */
        public int PageNo { get; set; } = 1;

        public int Start { get; set; }

        /**
    * 当前页的数据
    */
        public IList<T> Data { get; set; }
        /**
         * 
         */
        public SimplePage()
        {
        }
        /**
             * SimplePage的构造器，主要用于调整传入参数。
             * 
             * @param pageNo
             *            当前页码
             * @param pageSize
             *            分页大小
             * @param totalCount
             *            总记录数
             */
        public SimplePage(int pageNo, int pageSize, int totalCount, IList<T> data)
        {
            SetTotalCount(totalCount);
            SetPageSize(pageSize);
            SetPageNo(pageNo);
            this.Data = data;
            this.LastPage = this.IsLastPage();
            this.FirstPage = this.IsFirstPage();
            AdjustPageNo();
            this.Start = this.GetFirstIndex();
        }

        //public SimplePage(int pageNo, int pageSize, int totalCount, Boolean lastPage, IList<T> data)
        //{
        //    SetTotalCount(totalCount);
        //    SetPageSize(pageSize);
        //    SetPageNo(pageNo);
        //    AdjustPageNo();
        //    this.Data = data;
        //    this.FirstPage = this.IsFirstPage();
        //    this.LastPage = lastPage;
        //}
        /**
         * 用于处理页码。如果页码为空，或者小于1时，会造成分页程序处理 出现错误，当传入页码为空，或者小于1时，该方法返回固定值1，否则直接返回传入页码。
         * 
         * @param pageNo
         *            调整后的当前页码
         * @return
         */
        public static int ValidPageNumber(int pageNo)
        {
            return (pageNo < 1) ? 1 : pageNo;
        }

        /**
         * 用于调整当前页码，使当前页码不超过最大页数。
         * <p>
         * 该方法在构造函数SimplePage(int pageNo, int pageSize, int totalCount)中调用。 比如用
         * 使用SimplePage构造函数时传入的pageNo（当前页码）是10，但是最大页数是9，分页中不允许
         * 当前页码大于最大页数。所以需要使用该方法将10调整为9。
         * 
         */
        public void AdjustPageNo()
        {
            if (PageNo == 1)
            {
                return;
            }
            int tp = GetTotalPage();
            if (PageNo > tp)
            {
                PageNo = tp;
            }

        }

        /**
         * 用于调整总记录数，使总记录数不能小于0。
         * <p>
         * 该方法在构造函数SimplePage(int pageNo, int pageSize, int totalCount)中调用。
         * 比如用使用SimplePage构造
         * 函数时传入的totalCount（总记录数）是-1，但是在分页中不允许总记录数小于0。所以需要使用该方法-1调整为0。
         * 
         * @param totalCount
         *            调整后的总记录数
         */
        public void SetTotalCount(int totalCount)
        {
            if (totalCount < 0)
            {
                this.TotalCount = 0;
            }
            else
            {
                this.TotalCount = totalCount;
            }
        }

        /**
         * 用于调整分页大小，使分页大小不能于0。 如果小于0，则调整为默认常量值：DEF_COUNT。
         * <p>
         * 该方法在构造函数SimplePage(int pageNo, int pageSize, int totalCount)中调用。
         * 比如用使用SimplePage构造
         * 函数时传入的pageSize（分页大小）是-1，但是在分页中不允分页大小小于1。所以需要使用该方法-1调整为0。
         * 
         * @param pageSize
         *            调整后的分页大小
         */
        public void SetPageSize(int pageSize)
        {
            if (pageSize < 1)
            {
                this.PageSize = DEF_COUNT;
            }
            else
            {
                this.PageSize = pageSize;
            }
        }

        /// <summary>
        ///  用于调整当前页号，使当前页号不能小于1。如果小于1，则调整为1。
        ///  该方法在构造函数SimplePage(int pageNo, int pageSize, int totalCount)中调用。
        ///  比如用使用SimplePage构造 函数时传入的pageNo（当前页号）是0，但是在分页中不允许当前页号小于1。所以需要使用该方法-1调整为0。
        /// </summary>
        /// <param name="pageNo"></param>
        public void SetPageNo(int pageNo)
        {
            if (pageNo < 1)
            {
                this.PageNo = 1;
            }
            else
            {
                this.PageNo = pageNo;
            }
        }

        /// <summary>
        /// 获得页码
        /// </summary>
        /// <returns></returns>
        public int GetPageNo()
        {
            return PageNo;
        }

        /// <summary>
        /// 每页几条数据
        /// </summary>
        /// <returns></returns>
        public int GetPageSize()
        {
            return PageSize;
        }

        /// <summary>
        /// 总共几条数据
        /// </summary>
        /// <returns></returns>
        public int GetTotalCount()
        {
            return TotalCount;
        }

        /// <summary>
        /// 总共几页
        /// </summary>
        /// <returns></returns>
        public int GetTotalPage()
        {
            int totalPage = TotalCount / PageSize;
            if (totalPage == 0 || TotalCount % PageSize != 0)
            {
                totalPage++;
            }
            return totalPage;
        }

        /// <summary>
        /// 是否第一页
        /// </summary>
        /// <returns></returns>
        public Boolean IsFirstPage()
        {
            return PageNo <= 1;
        }

        /// <summary>
        /// 是否最后一页
        /// </summary>
        /// <returns></returns>
        public Boolean IsLastPage()
        {
            if (-1 == this.TotalCount)
            {
                return LastPage;
            }
            return PageNo >= GetTotalPage();
        }

        /// <summary>
        /// 下一页页码
        /// </summary>
        /// <returns></returns>
        public int GetNextPage()
        {
            if (IsLastPage())
            {
                return PageNo;
            }
            else
            {
                return PageNo + 1;
            }
        }

        /// <summary>
        /// 上一页页码
        /// </summary>
        /// <returns></returns>
        public int GetPrePage()
        {
            if (IsFirstPage())
            {
                return PageNo;
            }
            else
            {
                return PageNo - 1;
            }
        }
        /// <summary>
        ///  用于获得当前页数据中的开始数据所在总数据中的位置。
        /// <p>
        /// 例如分页数据50条，分页大小是5，第3页数据的开始数据所在的位置就是(3-1) * 5 = 10， 使用该方法获取的值就是10。
        /// <p>
        /// 该方法主要用于org.hibernate.Query接口的setFirstResult()。
        /// <p>
        /// 注意：数据位置是从0开始的！
        /// </summary>
        /// <returns>当前页数据中的开始数据所在总数据中的位置</returns>
        public int GetFirstIndex()
        {
            return (this.PageNo - 1) * this.PageSize;
        }
    }
}
