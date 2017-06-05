//********************************************************************  //
// Name:Coskun Olcucu                                                   //
//Project:Windows Form application for CRUD operations on SQL database. //
//        User can also search,reset a person's record by using form.   //
//Filename:Form1.cs                                                     //
//                                                                      //
//********************************************************************  //

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; //To call Stored Procedures 

namespace ProjectTest
{
    public partial class Form1 : Form
    {
        
        SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\joshua\Documents\Visual Studio 2017\Projects\DB\ContactDB.mdf;Integrated Security=True;Connect Timeout=30"); //created new sql connection by passing connection string 
        int PersonID = 0;  //initialize PersonId
        int EmailId = 0;  // initialize EmailId

        public Form1()
        {
            InitializeComponent();
        }

   
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        /**
          btnSave_Click method insert new record to the database table.
          btnSave_Click methos also update the existing record on the database table.  
        */
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)                                //Check Sql conncetion status
                    sqlCon.Open();                                                        //Open the sql connection

                if (btnSave.Text == "Save")                                               //if the user hit the save button on the form
                {

                    SqlCommand sqlCmd = new SqlCommand("PersonAddOrEdit", sqlCon);      //Initialize a new instance of the System.Data.SqlClient.SqlCommand class with the text of query and a sql connection
                    sqlCmd.CommandType = CommandType.StoredProcedure;                  //Specify how command string interpreted or used.
                    //Pass values in Stored Procedure  as parameters 
                    sqlCmd.Parameters.AddWithValue("@mode", "ADD");                   // Mode condition in Query to Add or Save email and person
                    sqlCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());    //Pass text box value txtName and Trim method removes extra left and right spaces on the text box.
                    sqlCmd.Parameters.AddWithValue("@Age", txtAge.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@EmailAddress", txtAddress.Text.Trim());
                    sqlCmd.ExecuteNonQuery();                                //Excecute the command by calling ExecuteNonQuery method that makes changes in database record.
                    MessageBox.Show("Saved successfully");                   //Show user a pop up message after saving new record.
                    
                }
                else  //if person wants to update a record
                {

                    SqlCommand sqlCmd = new SqlCommand("PersonAddOrEdit", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@mode", "EDIT");
                    sqlCmd.Parameters.AddWithValue("@PersonId", PersonID);
                    sqlCmd.Parameters.AddWithValue("@EmailId", EmailId);
                    sqlCmd.Parameters.AddWithValue("@PersonName", "");
                    sqlCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@Age", txtAge.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@EmailAddress", txtAddress.Text.Trim());
                    sqlCmd.ExecuteNonQuery();
                    MessageBox.Show("Updated successfully"); 

                }
                //Call Reset and FillDataGridview if form is opened first time.
                Reset();  
                FillDataGridView();
            }
            catch (Exception ex)                                                    //catch if there is any error
            {

                MessageBox.Show(ex.Message, "Error Message");                       //show error message


            }

            finally
            {

                sqlCon.Close();                                                    // close sql connection
            }

        } 

        /**
          FillDataGridView method is used for displaying the records name and age of person.       
        */
       void FillDataGridView()
         {
            if (sqlCon.State == ConnectionState.Closed)                   //Check Sql Connection if connection status close
                sqlCon.Open();                                            // open sql connection
            SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewOrSearch", sqlCon);                //initialize a new instance of the SqlDataAdapter class with parameters stored procedure and sql connection object.This object used for retrieve data from the database table.
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;                          //specify the command type to retrieve data from the sql table
            sqlDa.SelectCommand.Parameters.AddWithValue("@ContactName", txtSearch.Text.Trim());    // to get value in the search box, pass value in the stored procedure as a parameter.
            DataTable dtbl = new DataTable();                                                      //initialize instance of Datatable class in order to save the results that return from sql table.
            sqlDa.Fill(dtbl);                                                                     // filling data table with the results that return from sql table by calling Fill method
            dgvContacts.DataSource = dtbl;                                                       // show dtbl records  on the form in DataGridview.
            dgvContacts.Columns[0].Visible = false;                                             //Hide the Id column on DataGridView
            sqlCon.Close();                                                                    //close sql connection
        }
        /**
          Method FillDataGridView2 is used for displaying Email records that belongs one person.
         */
        void FillDataGridView2()
        {
            if (sqlCon.State == ConnectionState.Closed)                                           //check sql connection status
                sqlCon.Open();                                                                    //open connection
            SqlDataAdapter sqlDa = new SqlDataAdapter("PersonAddOrEdit", sqlCon);                 //initialize a new instance of SqlDataAdapter class with parameters stored procedure and a sql connection object
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;                        //Specify the CommandType to retrieve data from the sql table  
            sqlDa.SelectCommand.Parameters.AddWithValue("@mode", "");                             
            sqlDa.SelectCommand.Parameters.AddWithValue("@PersonName", txtSearch.Text.Trim());   //to get a person email records,pass person name as a parameter 
            DataTable dtbl2 = new DataTable();                                                   //initialize instance of DataTable class in order to save the results that return from sql table.
            sqlDa.Fill(dtbl2);                                                                   //fill the table with results
            dgvContacts2.DataSource = dtbl2;                                                     //show the results on the DataGridView2
            //Hide all the columns except Email to show only email records.
            dgvContacts2.Columns[0].Visible = false;                                            //make each column visibility to false.
            dgvContacts2.Columns[1].Visible = false;
            dgvContacts2.Columns[2].Visible = false;
            dgvContacts2.Columns[3].Visible = false;
            sqlCon.Close();                                                                   //close sql connection
        }

        /**
           btnSearch_Click Method displays the search results both 
           on DataGridview and DataGriview2 by calling FillDataGridview and FillDataGridview2
            
        */ 
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataGridView();                                                          //displays a table for one person's name and age record.
                FillDataGridView2();                                                        //displays a table for one person's email addresses
            }
            catch (Exception ex)                                                             //catch if any error occurs
            {
                MessageBox.Show(ex.Message, "Error Message");                               //display error message
            }
        }

       

        private void dgvContacts_CellContentClick(object sender, EventArgs e)
        {



        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {



        }
        
        /**
          dgvContacts_DoubleClick method is used for updating a record that is displayed on DataGridView.
          DataGridview displays only person's name and age record.
          User has to double click inside the DataGridview to update a record.
         */
       private void dgvContacts_DoubleClick(object sender, EventArgs e)
        {
            if (dgvContacts.CurrentRow.Index != -1)                                                  //check to see if it is clickable inside the DataGridView
            {
                //Populate textboxes Name and Age with appropriate data for updating the records when user double click on DataGridView
                PersonID = Convert.ToInt32(dgvContacts.CurrentRow.Cells[0].Value.ToString());         // Get the personID for updating the data for that person
                txtName.Text = dgvContacts.CurrentRow.Cells[1].Value.ToString();                      //gets the cell at the provided index location
                txtAge.Text = dgvContacts.CurrentRow.Cells[2].Value.ToString();
                btnSave.Text = "Update";                                                             //change the save button from "Save" to "Update" when click the DataGridView row.
                btnDelete.Enabled = true;                                                            // enable the delete button that it is disabled by default.

            }
        }
        /**
           Method Reset  clear all control values and reset variables to inital state.
         */ 

        void Reset()
        {
            txtName.Text = txtAge.Text = txtAddress.Text = "";                                    //Reset all text Boxes by assigning them empty string.(Name,Age,Email)
            btnSave.Text = "Save"; 
            PersonID = 0;
            btnDelete.Enabled = false;
        }
        /**
           Method btnReset_Click trigger the reset operation when the user hit reset button on the form by calling Reset method.
         */
        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
        /**
           Method Form1_Load  is triggerred when the first time form is open.
           When the form open,all the textboxes are inital state and all data records to be displayed
           by calling Reset and FillDataGridView methods. 
         */ 
        private void Form1_Load(object sender, EventArgs e)
        {
            Reset();
            FillDataGridView();

        }
        /**
           Method btnDelete_Click deletes a record from database table.
         */ 
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)                                     //check connection status
                    sqlCon.Open();                                                              //open Connection


                SqlCommand sqlCmd = new SqlCommand("PersonAddOrEdit", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@mode", "DELETE");                            //pass stored procedure operation parameter
                sqlCmd.Parameters.AddWithValue("@PersonId", PersonID);
                sqlCmd.Parameters.AddWithValue("@EmailId", EmailId);
                sqlCmd.Parameters.AddWithValue("@PersonName", "");
                sqlCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@Age", txtAge.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@EmailAddress", txtAddress.Text.Trim());
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("Deleted successfully");
                Reset();                                                                      //reset the form fields 
                FillDataGridView();                                                           //display the existing records.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message");
            }
        }

       private void dgvContacts2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        /**
          Method dgvContacts2_DoubleClick is used for updating email records that is displayed on DataGridView2.
          DataGridView2 displays email records that belong to one person.
          User has to double click DataGridView2
         */
        private void dgvContacs2_DoubleClick(object sender, EventArgs e)
        {
            if (dgvContacts2.CurrentRow.Index != -1)                                                //check to see if DataGridView2 row clickable
            {
                //Populate textboxe name Email with appropriate email addresses
                PersonID = Convert.ToInt32(dgvContacts2.CurrentRow.Cells[0].Value.ToString());     // To Get PersonId to update or detele that person data
                EmailId = Convert.ToInt32(dgvContacts2.CurrentRow.Cells[1].Value.ToString());      // To Get the EmailId to update or delete that particular email of the person
                txtName.Text = dgvContacts2.CurrentRow.Cells[2].Value.ToString();
                txtAge.Text =  dgvContacts2.CurrentRow.Cells[3].Value.ToString();
                txtAddress.Text = dgvContacts2.CurrentRow.Cells[4].Value.ToString();

                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }

        }

      private void dgvContacts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }//End of Class
}