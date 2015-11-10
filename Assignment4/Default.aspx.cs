using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
//using tagLib;
using System.IO;
using System.Data.SqlClient;

public partial class _Default : System.Web.UI.Page
{
    //connection string
    protected const string connstr = "server=10.158.56.48;uid=net5;pwd=dtbz5;database=notebase5;";
    protected SqlCommand deleteCommand;
    protected SqlCommand insertCommand;
    protected SqlCommand updateCommand;
    protected SqlConnection connection;
    protected DataSet dset;
    protected DataSet tag_dset;
    protected DataSet id_dset;
    protected SqlDataAdapter dataAdapter;
    protected DataTable NoteTable;
    protected DataTable TagsTable;
    protected DataTable IdTable;
    protected DataRow SelectedDataRow;
    protected int deleted = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        refreshGrid();
    }
    protected void add_btn_Click(object sender, EventArgs e)
    {
        changeNote();

        //Show the user that the note was added.
        clearTextBoxes();
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        //gets the sender object data.
        GridView dgv = sender as GridView;
        //checks to make sure it wasn't empty/no selection
        if (dgv != null )//&& dgv.SelectedRow > 0)
        {
            //grabs the row data into a row object.
            GridViewRow row = dgv.SelectedRow;
            if (row != null)
            {
                //displays all the row data into the textboxes.
                Title_TextBox.Text = row.Cells[1].ToString();

                int count = 0;
                List<string> TL = new List<string>();
                foreach (DataRow idrow in IdTable.Rows)
                {

                    //need something to check the id in the tags table. 
                    //need to iterate through the tags table and check tagID's
                    if (IdTable.Rows[count][0].ToString() == row.Cells[0].ToString())
                    {
                        foreach (DataRow dr in TagsTable.Rows)
                        {
                            //check if the id's tagid is the same as the tag's tagid
                            if ((int)IdTable.Rows[count][1] == (int)dr[0])
                            {
                                TL.Add(dr[1].ToString());
                            }
                        }
                    }
                    count++;
                }
                Tags_TextBox.Text = "";
                //TODO: make a fucntion to do this. 
                foreach (string ele in TL)
                {
                    if (ele.Equals(TL.Last()))
                        Tags_TextBox.Text += ele;
                    else
                        Tags_TextBox.Text += ele + ":";
                }
                Text_TextBox.Text = row.Cells[2].ToString();
            }
        }
    }

    public DataSet CreateDataset(string Table)
    {
        string cmdString = "Select * from " + Table;

        //create dataset command object and the Dataset
        dataAdapter = new SqlDataAdapter(cmdString, connstr);
        DataSet dataSet = new DataSet();

        //fill the dataset object
        dataAdapter.Fill(dataSet, Table);
        dataAdapter.Dispose();

        return dataSet;
    }

    public DataSet CreateViewset()
    {
        string cmdString = @"SELECT     Notes.Title, Tags.Tag, Notes.TextBody, Notes.TimeCreated, Notes.TimeUpdated
                      FROM         Notes INNER JOIN
                      IDs ON Notes.NoteID = IDs.NoteID INNER JOIN
                      Tags ON IDs.TagID = Tags.TagID";

        dataAdapter = new SqlDataAdapter(cmdString, connstr);
        DataSet dataSet = new DataSet();

        dataAdapter.Fill(dataSet);
        dataAdapter.Dispose();

        return dataSet;
    }

    //This function is used to grab the data from the database tables. 
    public void refreshGrid()
    {
        //call a method to return a data set
        dset = CreateDataset("Notes");

        //set the DataGrid's data source
        NoteTable = dset.Tables[0];

        tag_dset = CreateDataset("Tags");
        TagsTable = tag_dset.Tables[0];
        id_dset = CreateDataset("IDs");
        IdTable = id_dset.Tables[0];


        GridView1.DataSource = CreateViewset();
        GridView1.DataBind();
    }

    private void con_update()
    {

        connection = new SqlConnection(connstr);
        dataAdapter = new SqlDataAdapter("Select * from Notes", connection);

        connection.Open();

        cbuilder("Notes", dset);

        dataAdapter = new SqlDataAdapter("Select * from Tags", connection);
        cbuilder("Tags", tag_dset);

        dataAdapter = new SqlDataAdapter("Select * from IDs", connection);
        cbuilder("IDs", id_dset);

        connection.Close();

    }

    //Calls the ID tables to update it
    private void con_delete()
    {
        connection = new SqlConnection(connstr);

        connection.Open();

        dataAdapter = new SqlDataAdapter("Select* from IDs", connection);
        cbuilder("IDs", id_dset);

        connection.Close();
    }

    //this builds commands to update, insert, or delete from the database.
    private void cbuilder(string table, DataSet cb_dset)
    {
        SqlCommandBuilder builder = new SqlCommandBuilder(dataAdapter);
        dataAdapter.UpdateCommand = builder.GetUpdateCommand();
        //MessageBox.Show(dataAdapter.UpdateCommand.CommandText);
        dataAdapter.InsertCommand = builder.GetInsertCommand();
        //MessageBox.Show(dataAdapter.InsertCommand.CommandText);
        dataAdapter.DeleteCommand = builder.GetDeleteCommand();
        //MessageBox.Show(dataAdapter.DeleteCommand.CommandText);

        try
        {
            dataAdapter.RowUpdated += new SqlRowUpdatedEventHandler(dataAdapter_RowUpdated);
            int rowsupdated = dataAdapter.Update(cb_dset, table);
            dataAdapter.RowUpdated -= new SqlRowUpdatedEventHandler(dataAdapter_RowUpdated);

        }
        catch (Exception ex)
        {
            //MessageBox.Show(ex.Message);
            Response.Write("<script>alert('" + ex.Message + "')</script>");
        }
    }

    private void dataAdapter_RowUpdated(object sender, SqlRowUpdatedEventArgs e)
    {
        string s = System.Enum.GetName(e.StatementType.GetType(), e.StatementType);
        //MessageBox.Show(s);
        Response.Write("<script>alert('" + s + "')</script>");
        if (e.RecordsAffected < 1)
        {
            Response.Write("<script>alert(Concurrency Problem occurred)</script>");
            //MessageBox.Show("Concurrency Problem occurred");
            e.Status = UpdateStatus.SkipCurrentRow;
        }
    }

    private void changeNote()
    {

        if (Tags_TextBox.Text == "" || Title_TextBox.Text == "" || Text_TextBox.Text == "")
        {
            //Displays a message if nothing is entered.

            if (Title_TextBox.Text == "")
                Response.Write("<script>alert(Please enter a title for your note.)(No Title)</script>");
            //MessageBox.Show("Please enter a title for your note.", "No Title");
            if (Tags_TextBox.Text == "")
                Response.Write("<script>alert(Please enter tags seperated by : \n ex. Tags:Tag:Note)</script>");
            //MessageBox.Show("Please enter tags seperated by : \n ex. Tags:Tag:Note", "Tags Error");
            if (Text_TextBox.Text == "")
                Response.Write("<script>alert(Please the body of your text.)(No Body)</script>");
            //MessageBox.Show("Please the body of your text.", "No Body");
        }

        else
        {
           
                //New tagsplit object
                //TagSplit TS = new TagSplit();
                DateTime timenow = DateTime.Now;

                //Send the tags to the tagslitter
                //List<string> tagList = TS.tagger(Tags_TextBox.Text);

                //Make a new note object
                //NoteItem NI = new NoteItem(Text_TextBox.Text, Title_TextBox.Text, tagList);

                //database magic
                DataRow newRow = NoteTable.NewRow();
                newRow["Title"] = Title_TextBox.Text;
                newRow["TextBody"] = Text_TextBox.Text;
                newRow["TimeUpdated"] = timenow;
                newRow["TimeCreated"] = timenow;
                NoteTable.Rows.Add(newRow);



                int tag_row = TagsTable.Rows.Count;
                //checks for null values in tags table because previous tags were deleted. 
                while (TagsTable.Rows[tag_row - 1][0] == DBNull.Value)
                { tag_row--; }
                tag_row = Convert.ToInt32(TagsTable.Rows[tag_row - 1][0]);

                int note_row = NoteTable.Rows.Count;
                //checks for null values in tags table because previous tags were deleted. 
                while (NoteTable.Rows[note_row - 1][0] == DBNull.Value)
                { note_row--; }
                note_row = Convert.ToInt32(NoteTable.Rows[note_row - 1][0]);

                //foreach (string tag in tagList)
                {
                    DataRow tagRow = TagsTable.NewRow();
                    //tagRow["Tag"] = tag;
                    TagsTable.Rows.Add(tagRow);
                    DataRow idRow = IdTable.NewRow();
                    //note id is wrong, need to check for null values.
                    idRow["NoteID"] = note_row + 1;

                    idRow["TagID"] = tag_row + 1;
                    IdTable.Rows.Add(idRow);
                    tag_row++;
                }


                //Show the user that the note was added.
                //MessageBox.Show("Note added", "New Note");
                Response.Write("<script>alert(Note added)(New Note)</script>");

                //Update the data grid.
                con_update();
                refreshGrid();
                //dataGridView1.Update();
                //dataGridView1.Refresh();
            
        }
    }

    public void clearTextBoxes()
    {
        Title_TextBox.Text = Tags_TextBox.Text = Text_TextBox.Text = "";
    }

    protected void Button1_Click(object sender, EventArgs e)
    {

    }
    protected void Delete_Button_Click(object sender, EventArgs e)
    {
        int count = 0;
        //checks for something to be selected.
        //if (GridView1.SelectedRow > 0)
        //{
            //remove all the tags related
            foreach (DataRow idrow in IdTable.Rows)
            {
                //if the note ids match
                //if (IdTable.Rows[count][0].ToString() == GridView1.SelectedRow[0].Cells[0].Value.ToString())
                {
                    //go through tags table 
                    for (int i = TagsTable.Rows.Count - 1; i >= 0; i--)
                    {
                        if ((int)IdTable.Rows[count][1] == (int)TagsTable.Rows[i][0])
                        {
                            TagsTable.Rows[i].Delete();
                            TagsTable.AcceptChanges();
                        }
                    }
                    //TagsTable.Rows[(int)IdTable.Rows[count][1]].Delete();
                    IdTable.Rows[count].Delete();
                }
                count++;
            }
            //remove the selected row.
            //delete the row
            GridView1.DeleteRow(GridView1.SelectedIndex);
            //TODO MAKE ERORR LABLES
            //clears the text boxes.
            clearTextBoxes();
            //delete the id from the idtable before it deletes it from the noteID table. 
            con_delete();
            con_update();
            refreshGrid();
        //}
        //else
            //let user know, no row was selected.
            //MessageBox.Show("please select note to be deleted.", "no selection.");
            //Response.Write("<script>alert(please select note to be deleted)(No Selection)</script>");
            //int count = 0;
            //checks for something to be selected.
            //if (GridView1.SelectedRow. > 0)
            //{
                //remove all the tags related
                //foreach (DataRow idrow in IdTable.Rows)
                {
                    //if the note ids match
                    //if (IdTable.Rows[count][0].ToString() == GridView1.SelectedRow[0].Cells[0].Value.ToString())
                    {
                        //go through tags table 
                        for (int i = TagsTable.Rows.Count - 1; i >= 0; i--)
                        {
                            if ((int)IdTable.Rows[count][1] == (int)TagsTable.Rows[i][0])
                            {
                                TagsTable.Rows[i].Delete();
                                TagsTable.AcceptChanges();
                            }
                        }
                        //TagsTable.Rows[(int)IdTable.Rows[count][1]].Delete();
                        IdTable.Rows[count].Delete();
                    }
                    count++;
                }
                //remove the selected row.
                //todo: remove it's tags as well?

                /*foreach (GridViewRow item in this.GridView1.SelectedRow)
                {
                    
                    dataGridView1.Rows.RemoveAt(item.Index);
                   
                }*/
                //clears the text boxes.
                clearTextBoxes();
                //delete the id from the idtable before it deletes it from the noteID table. 
                con_delete();
                con_update();
                refreshGrid();
            //}
            //else
                //let user know, no row was selected.
                //MessageBox.Show("please select note to be deleted.", "no selection.");
                //Response.Write("<script>alert(Please select a note to be deleted)(No Selection)</script>");
    }
    protected void Edit_Button_Click(object sender, EventArgs e)
    {
        //check for something selected. 
        //if (GridView1.SelectedRow.Count > 0)
        {
            //go through table.
            foreach (DataRow dr in NoteTable.Rows)
            {
                //TODO: edit tags as well.
                //if (GridView1.SelectedRow[0].Cells[0].Value.ToString() == dr[0].ToString())
                {
                    DateTime timenow = DateTime.Now;
                    DataRow newRow = NoteTable.NewRow();
                    dr["Title"] = Title_TextBox.Text;
                    dr["TextBody"] = Text_TextBox.Text;
                    dr["TimeUpdated"] = timenow;

                    int count = 0;

                    //New tagsplit object
                    //TagSplit TS = new TagSplit();
                    //List<string> TL = TS.tagger(Tags_TextBox.Text);
                    //remove all the tags related
                    //go through tags table 

                    foreach (DataRow idrow in IdTable.Rows)
                    {
                        //if the note ids match
                        //if (IdTable.Rows[count][0].ToString() == GridView1.SelectedRow[0].Cells[0].Value.ToString())
                        {

                            //TagsTable.Rows[(int)IdTable.Rows[count][1]].Delete();
                            for (int i = TagsTable.Rows.Count - 1; i >= 0; i--)
                            {
                                if (TagsTable.Rows[i].RowState != DataRowState.Deleted && (int)IdTable.Rows[count][1] == (int)TagsTable.Rows[i][0])
                                {
                                    TagsTable.Rows[i].Delete();
                                    deleted++;
                                }
                            }
                            IdTable.Rows[count].Delete();
                        }
                        count++;
                    }

                    //int noteid = (int)GridView1.SelectedRow[0].Cells[0].Value;
                    con_delete();
                    con_update();
                    refreshGrid();

                    //add all the tags back.
                    int tag_row = TagsTable.Rows.Count;
                    //checks for null values in tags table because previous tags were deleted. 
                    while (TagsTable.Rows[tag_row - 1][0] == DBNull.Value)
                    { tag_row--; }
                    tag_row = Convert.ToInt32(TagsTable.Rows[tag_row - 1][0]);

                    //foreach (string tag in TL)
                    {
                        DataRow tagRow = TagsTable.NewRow();
                       // tagRow["Tag"] = tag;
                        TagsTable.Rows.Add(tagRow);
                        DataRow idRow = IdTable.NewRow();
                        //idRow["NoteID"] = noteid;
                        //this is currently incorrect and does not point to the correct tag. 

                        idRow["TagID"] = tag_row + 1 + deleted;
                        IdTable.Rows.Add(idRow);
                        tag_row++;
                    }
                    clearTextBoxes();
                    con_update();
                    refreshGrid();
                    //MessageBox.Show("Note Edited", "Changed");
                    Response.Write("<script>alert(Note Edited)(Changed)</script>");
                    break;
                }
            }
        }
        //else
            //if nothing is selected, tell user.
            //MessageBox.Show("please select note to be edited.", "no selection.");
            //Response.Write("<script>alert(Please select note to be edited)(No Selection)</script>");
    }
    protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
    {

    }
}