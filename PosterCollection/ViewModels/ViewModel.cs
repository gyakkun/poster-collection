using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosterCollection.Models;

namespace PosterCollection.ViewModels
{
    class ViewModel
    {
        private ObservableCollection<Result> queryResults = new ObservableCollection<Result>();
        private PosterDetail detail;

        private static ViewModel instance;

        public static ViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ViewModel();
                }
                return instance;
            }
        }

        public ObservableCollection<Result> QueryResults
        {
            get { return queryResults; }
            set { queryResults = value; }
        }

        public PosterDetail Detail
        {
            get { return detail; }
            set { detail = value; }
        }

        public void AddResult(Result result)
        {
            queryResults.Add(result);
        }
    }
}
