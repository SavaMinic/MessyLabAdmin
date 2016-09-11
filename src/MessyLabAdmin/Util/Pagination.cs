using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;

namespace MessyLabAdmin.Util
{
    public static class Pagination
    {

        public static IQueryable<T> ToPagedList<T>(this IQueryable<T> list, int page, int countPerPage)
        {
            return list.Skip((page-1)*countPerPage).Take(countPerPage);
        }
    }

    public class PaginationViewData
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }

        public int? PreviousPage { get; private set; }
        public int? NextPage { get; private set; }

        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public string SelectedAction { get; private set; }

        public PaginationViewData(int currentPage, int totalPages, string selectedAction = "Index")
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            SelectedAction = selectedAction;

            if (CurrentPage > 1) PreviousPage = CurrentPage - 1;
            if (CurrentPage < TotalPages) NextPage = CurrentPage + 1;

            StartPage = CurrentPage - 5;
            EndPage = CurrentPage + 4;
            if (StartPage <= 0)
            {
                EndPage -= StartPage - 1;
                StartPage = 1;
            }
            if (EndPage > TotalPages)
            {
                EndPage = TotalPages;
                if (EndPage > 10)
                {
                    StartPage = EndPage - 9;
                }
            }
        }
    }
}
