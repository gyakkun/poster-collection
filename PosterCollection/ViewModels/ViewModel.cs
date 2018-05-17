using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosterCollection.Models;
using SQLitePCL;

namespace PosterCollection.ViewModels
{
    class ViewModel
    {
        private ObservableCollection<MovieResult> queryMovieResults = new ObservableCollection<MovieResult>();
        private ObservableCollection<TVResult> queryTVResults = new ObservableCollection<TVResult>();
        private ObservableCollection<Star> starlist = new ObservableCollection<Star>();
        private MovieDetail theMovieDetail = new MovieDetail();
        private TVDetail theTVDetail = new TVDetail();

        private static ViewModel instance;

        private ViewModel()
        {
            SQLiteConnection db = App.conn;
            using (var statement = db.Prepare(App.SQL_QUERY_VALUE))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    
                    Starlist.Add(new Star(Convert.ToInt32(statement[0]), (string)statement[1], (string)statement[2], (string)statement[3]));
                    
                }
            }

        }

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

        public ObservableCollection<Star> Starlist
        {
            get { return starlist; }
            set { starlist = value; }
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
        public void AddStar(Star st)
        {

            starlist.Add(st);
            
            var db = App.conn;

            try
            {
                using (var TodoItem = db.Prepare(App.SQL_INSERT))
                {
                    TodoItem.Bind(1, st.id);
                    TodoItem.Bind(2, st.title);
                    TodoItem.Bind(3, st.imagepath);
                    TodoItem.Bind(4, st.comment);
                    
                    TodoItem.Step();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void clear()
        {
            queryMovieResults.Clear();
            queryTVResults.Clear();
            starlist.Clear();
        }
    }
}
