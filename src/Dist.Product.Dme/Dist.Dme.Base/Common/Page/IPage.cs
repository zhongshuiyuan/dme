using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Common.Page
{
    public interface IPage
    {
        /**
     * 获取分页数据的总记录数。例如User表中有100条数据， 要分页查询“年龄大于20的user“。此时如果
     * User表中年龄大于20的数据有50条，此时这50条数 据就是我们所讲的分页数据，该方法得到的分页数据总记录数就是50。
     * 
     * @return 分页数据的总记录数
     */
        int GetTotalCount();

        /**
         * 用于获取分页数据的总页数。例如分页数据有50条，每一页 显示5条数据，那么需要10才能显示完50条数据，该方法得到的分页数据总页数就是10
         * 
         * @return 分页数据的总页数
         */
        int GetTotalPage();

        /**
         * 用于获取每一页显示的数据总数。例如分页数据有50条，每一页 显示了5条数据，那么该方法得到的每一页显示的数据总数就是5。
         * 
         * @return 每一页显示的数据总数
         */
        int GetPageSize();

        /**
         * 用于获取当前页的页号。例如分页总数有50页，如果 当前页是第10页，该方法得到的当前页号就是10。
         * 
         * @return 当前页号
         */
        int GetPageNo();

        /**
         * 用于判断当前页是否是分页总数的第一页。
         * 
         * @return 是第一页则返回true，否则返回false
         */
        Boolean IsFirstPage();

        /**
         * 用于判断当前页是否是分页总数最后一页。
         * 
         * @return 是最后一页则返回true，否则返回false
         */
        Boolean IsLastPage();

        /**
         * 用于获取当前页的下一页的页号。例如当前页是第10页， 它的下一页是11，该方法获得的当前页的下一页的页号就是11。
         * 
         * @return 当前页的下一页的页号
         */
        int GetNextPage();

        /**
         * 用于获取当前页的上一页的页号。例如当前页是第10页， 它的上一页是9，该方法获得的当前页的上一页的页号就是9。
         * 
         * @return 当前页的上一页的页号
         */
        int GetPrePage();
    }
}
