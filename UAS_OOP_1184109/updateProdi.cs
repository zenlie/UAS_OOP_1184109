﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace UAS_OOP_1184109
{
    public partial class updateProdi : Form
    {
        public updateProdi()
        {
            InitializeComponent();
        }

            private DataSet ds_Prodi;

            public DataSet CreateProdiDataSet()
            {
                DataSet myDataSet = new DataSet();

                try
                {

                    SqlConnection myConnection = new SqlConnection(@"Data Source=DESKTOP-GG3TQA3\SQLEXPRESS; Initial Catalog = UAS; Integrated Security = True");


                    SqlCommand myCommand = new SqlCommand();


                    myCommand.Connection = myConnection;


                    myCommand.CommandText = "SELECT * FROM ms_prodi";
                    myCommand.CommandType = CommandType.Text;


                    SqlDataAdapter myDataAdapter = new SqlDataAdapter();
                    myDataAdapter.SelectCommand = myCommand;
                    myDataAdapter.TableMappings.Add("Table", "Prodi");


                    myDataAdapter.Fill(myDataSet);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                return myDataSet;
            }

            private void RefreshDataSet()
            {

                ds_Prodi = CreateProdiDataSet();

                dgProdi.DataSource = ds_Prodi.Tables["Prodi"];
            }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDataSet();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // connection string digunakan untuk koneksi ke basisdata UAS
                 SqlConnection myConnection = new SqlConnection(@"Data Source=DESKTOP-GG3TQA3\SQLEXPRESS; Initial Catalog = UAS; Integrated Security = True");

            //buka connection
            myConnection.Open();

            //membuat dataadapter dan commandbuilder
            SqlDataAdapter myAdapter = new SqlDataAdapter("select * from ms_prodi", myConnection);
            SqlCommandBuilder myCmdBuilder = new SqlCommandBuilder(myAdapter);

            //menggunakan commandbuilder untuk build insertcommand, updatecommand, dan
            //deletecommand required by dataadapter
            myAdapter.InsertCommand = myCmdBuilder.GetInsertCommand();
            myAdapter.UpdateCommand = myCmdBuilder.GetUpdateCommand();
            myAdapter.DeleteCommand = myCmdBuilder.GetDeleteCommand();

            //perhatikan juga mengenai transaksi.. ini sangat penting!
            //Pelajari..!
            SqlTransaction myTransaction;
            myTransaction = myConnection.BeginTransaction();
            myAdapter.DeleteCommand.Transaction = myTransaction;
            myAdapter.UpdateCommand.Transaction = myTransaction;
            myAdapter.InsertCommand.Transaction = myTransaction;

            //coba untuk update, jika sukses commit
            //jika tidak roll back
            try
            {
                //panggil method update dari DataAdapter
                //dan menyimpan jumlah perubahan baris datanya ke variable rowsUpdate
                int rowsUpdated = myAdapter.Update(ds_Prodi, "Prodi");

                //method commit, bagian penting dari transaksi
                //jika ada kegagalan dalam satu proses transaksi, maka seluruh transaksi dibatalkan
                myTransaction.Commit();

                //tampilkan pesan jumlah baris data yang diupdate ke layar
                MessageBox.Show(rowsUpdated.ToString() + "Baris diperbarui", "Informasi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                //panggil method RefreshDataSet untuk menyegarkan datagris
                RefreshDataSet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to update: " + ex.Message);
                //jika terjadi kegagalan dalam transaksi, batalkan semua (rollback)
                myTransaction.Rollback();
            }

            //coba hilangkan coment dari baris berikut, untuk mengetahui command yang dibuat 
            //oleh sqlCommandBuilder
            MessageBox.Show(myAdapter.InsertCommand.CommandText);
            MessageBox.Show(myAdapter.UpdateCommand.CommandText);
        }
    
    }
}
