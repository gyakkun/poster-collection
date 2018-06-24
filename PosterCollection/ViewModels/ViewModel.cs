using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosterCollection.Models;
using PosterCollection.Service;
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
        private ObservableCollection<UsersInfo> usersList = new ObservableCollection<UsersInfo>();
        public int  currentUid;

        private static ViewModel instance;

        private ViewModel()
        {
            SQLiteConnection db = App.conn;
            
            using (var statement = db.Prepare(App.SQL_QUERY_USER))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    usersList.Add(new UsersInfo(Convert.ToInt32(statement[0]), (string)statement[1], (string)statement[2], (string)statement[3], (string)statement[4], Convert.ToInt32(statement[5])));
                }
            }

            
            
        }
        public void initcollection()
        {
            SQLiteConnection db = App.conn;
            using (var statement = db.Prepare(App.SQL_QUERY_VALUE))
            {

                statement.Bind(1, currentUid);
                while (SQLiteResult.ROW == statement.Step())
                {

                    Starlist.Add(new Star(Convert.ToInt32(statement[1]), (string)statement[2], (string)statement[3], (string)statement[4], (string)statement[5], Convert.ToInt32(statement[6])));

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

        public ObservableCollection<UsersInfo> UsersList
        {
            get { return usersList; }
            set { usersList = value; }
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

            var db = App.conn;

            try
            {
                using (var Item = db.Prepare("INSERT INTO Movies (Id,Title,overview,Type) VALUES(?,?,?,?);"))
                {
                    Item.Bind(1, result.id);
                    Item.Bind(2, result.title);
                    Item.Bind(3, result.overview);
                    Item.Bind(4, 0);
                 
                    Item.Step();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void AddTVResult(TVResult result)
        {
            queryTVResults.Add(result);

            var db = App.conn;

            try
            {
                using (var Item = db.Prepare("INSERT INTO Movies (Id,Title,overview,Type) VALUES(?,?,?,?);"))
                {
                    Item.Bind(1, result.id);
                    Item.Bind(2, result.name);
                    Item.Bind(3, result.overview);
                    Item.Bind(4, 1);

                    Item.Step();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void DeleteStar(int id,int type)
        {
            
            for (int i = 0; i < starlist.Count; i++)
            {
                if (starlist[i].id == id && starlist[i].type == type)
                {
                    starlist.RemoveAt(i);
                    break;
                }
            }
            TileService.GenerateTiles();
            using (var statement = App.conn.Prepare(App.SQL_DELETE))
            {
                statement.Bind(1, id);
                statement.Bind(2, type);
                statement.Step();
            }
        }
        public void EditComment(int id,string comment,int type)
        {
            for (int i = 0; i < starlist.Count; i++)
            {
                if (starlist[i].id == id&&starlist[i].type == type)
                {
                    starlist[i].comment=comment;
                    TileService.GenerateTiles();
                    break;
                }
            }
            var db = App.conn;
            using (var Item = db.Prepare(App.SQL_UPDATE))
            {
                Item.Bind(1, comment);
                Item.Bind(2, id);
                Item.Bind(3, type);
                Item.Step();
            }

        }
        public void AddStar(Star st)
        {

            starlist.Add(st);
            TileService.GenerateTiles();
            var db = App.conn;

            try
            {
                using (var Item = db.Prepare(App.SQL_INSERT))
                {
                    Item.Bind(1, st.id);
                    Item.Bind(2, st.title);
                    Item.Bind(3, st.imagepath);
                    Item.Bind(4, st.posterpath);
                    Item.Bind(5, st.comment);
                    Item.Bind(6, st.type);
                    Item.Bind(7, currentUid);
                    Item.Step();
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

        public void createUser(UsersInfo user)
        {
            usersList.Add(user);
            var db = App.conn;

            using (var Item = db.Prepare(App.SQL_INSERT_USER))
            {
                Item.Bind(1, user.Username);
                Item.Bind(2, user.Password);
                Item.Bind(3, user.Email);
                Item.Bind(4, user.Phone);
                Item.Bind(5, user.Role);
                Item.Step();
            }
        }

        public void deleteUser(int id)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].Id == id)
                {
                    usersList.RemoveAt(i);
                    break;
                }
            }
            using (var statement = App.conn.Prepare(App.SQL_DELETE_USER))
            {
                statement.Bind(1, id);
                statement.Step();
            }
        }

        public void updateUser(UsersInfo user)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].Id == user.Id)
                {
                usersList[i] = user;
                break;
            }
        }
            using (var statement = App.conn.Prepare(App.SQL_UPDATE_USER))
            {
                statement.Bind(1, user.Username);
                statement.Bind(2, user.Password);
                statement.Bind(3, user.Email);
                statement.Bind(4, user.Phone);
                statement.Bind(5,  user.Id);
                statement.Step();
            }
        }
    }
}
