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
                    
                    Starlist.Add(new Star(Convert.ToInt32(statement[0]), (string)statement[1], (string)statement[2], (string)statement[3], Convert.ToInt32(statement[4])));
                    
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
        public void DeleteStar(int id)
        {
            
            for (int i = 0; i < starlist.Count; i++)
            {
                if (starlist[i].id == id)
                {
                    starlist.RemoveAt(i);
                    break;
                }
            }

            using (var statement = App.conn.Prepare(App.SQL_DELETE))
            {
                statement.Bind(1, id);
                
                statement.Step();
            }
        }
        public void EditComment(int id,string comment)
        {
            for (int i = 0; i < starlist.Count; i++)
            {
                if (starlist[i].id == id)
                {
                    starlist[i].comment=comment;
                    break;
                }
            }

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
                    TodoItem.Bind(5, st.type);
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
          
        }
    }
}
