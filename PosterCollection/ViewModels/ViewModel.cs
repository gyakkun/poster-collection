﻿using System;
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
        //电影列表
        private ObservableCollection<MovieResult> queryMovieResults = new ObservableCollection<MovieResult>();
        //电视剧列表
        private ObservableCollection<TVResult> queryTVResults = new ObservableCollection<TVResult>();
        //收藏列表
        private ObservableCollection<Star> starlist = new ObservableCollection<Star>();
        //某电影详情
        private MovieDetail theMovieDetail = new MovieDetail();
        //某电视剧详情
        private TVDetail theTVDetail = new TVDetail();

        private static ViewModel instance;

        private ViewModel()
        {
            SQLiteConnection db = App.conn;
            using (var statement = db.Prepare(App.SQL_QUERY_VALUE))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    
                    Starlist.Add(new Star(Convert.ToInt32(statement[0]), (string)statement[1], (string)statement[2], (string)statement[3], (string)statement[4], Convert.ToInt32(statement[5])));
                    
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
    }
}
