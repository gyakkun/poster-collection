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
        private ObservableCollection<MovieResult> queryMovieResults = new ObservableCollection<MovieResult>();
        private ObservableCollection<TVResult> queryTVResults = new ObservableCollection<TVResult>();
        private MovieDetail theMovieDetail = new MovieDetail();
        private TVDetail theTVDetail = new TVDetail();

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

        public ObservableCollection<MovieResult> QueryMovieResults
        {
            get { return queryMovieResults; }
            set { queryMovieResults = value; }
        }

        public ObservableCollection<TVResult> QueryTVResults
        {
            get { return queryTVResults; }
            set { queryTVResults = value; }
        }

        public MovieDetail TheMovieDetail
        {
            get { return theMovieDetail; }
            set { theMovieDetail = value; }
        }

        public TVDetail TheTVDetail
        {
            get { return theTVDetail; }
            set { theTVDetail = value; }
        }

        public void AddMovieResult(MovieResult result)
        {
            queryMovieResults.Add(result);
        }

        public void AddTVResult(TVResult result)
        {
            queryTVResults.Add(result);
        }

        public void clear()
        {
            queryMovieResults.Clear();
            queryTVResults.Clear();
        }
    }
}
