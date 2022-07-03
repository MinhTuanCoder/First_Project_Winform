using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Quan_Ly_Quan_An
{
    public partial class GoiMon : Form
    {
        public static GoiMon instance;
        public GoiMon()
        {
            InitializeComponent();
            instance = this;
        }
        int indexFood=-1,indexDrink=-1; //lấy chỉ số khi nhấn vào datagridview
        bool checkPay=false;
        Random random = new Random();
        DataTable dtMenuFood, dtMenuDrink, dtOrderDrink, dtGetPrice, dtGetIdSTTC, dtStatistical;
        DataTable dtOrderFood = new DataTable();
        string chuoiketnoi = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=QLQuanAn;Integrated Security=True";
        SqlConnection conn = null;
     
        SqlDataAdapter daMenuFood = new SqlDataAdapter();
        SqlDataAdapter daMenuDrink = new SqlDataAdapter();
        SqlDataAdapter daBill = new SqlDataAdapter();
        SqlDataAdapter daOrderFood = new SqlDataAdapter();
        SqlDataAdapter daOrderDrink = new SqlDataAdapter();
        SqlDataAdapter daExspendables = new SqlDataAdapter();
        SqlDataAdapter daStatistical = new SqlDataAdapter();
        SqlDataAdapter daGetPrice =new SqlDataAdapter();
        SqlDataAdapter daGetIdSTTC = new SqlDataAdapter();

        private void dgvOrderFood_DataMemberChanged(object sender, DataGridViewCellEventArgs e)
        {
            int total = 0;
            for (int i = 0; i < dgvOrderFood.Columns.Count; i++)
            {
                total += Convert.ToInt32(dgvOrderFood.Rows[i].Cells[3].Value);

            }
            for (int i = 0; i < dgvOrderDrink.Columns.Count; i++)
            {
                total += Convert.ToInt32(dgvOrderDrink.Rows[i].Cells[3].Value);
            }
            textBox2.Text = total.ToString();
        }
        private void refreshCBB()
        {
            //Reset ComboBox chọn đồ ăn đồ uống
           
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;

        }


        private void GoiMon_Load(object sender, EventArgs e)
        {
            //Tạo liên kết tới SQL
            conn = new SqlConnection(chuoiketnoi);
            conn.Open();
            //Load form kết nối dữ liệu các món ăn tới combobox
            string sql_food = "select ID_Food as N'Mã món', Name_Food as N'Tên món', Price_Food as N'Giá tiền' from MENU_FOOD order by Name_Food; ";
            daMenuFood = new SqlDataAdapter(sql_food, conn);
            dtMenuFood = new DataTable();
            daMenuFood.Fill(dtMenuFood);
           
            cbbMenu_Food.DataSource = dtMenuFood;
            cbbMenu_Food.DisplayMember = "Tên món";
            cbbMenu_Food.ValueMember = "Mã món";
            //lấy dữ liệu cho combobox đồ uống
            string sql_drink = "select ID_Drink as N'Mã món', Name_Drink as N'Tên món', Price_Drink as N'Giá tiền'  from MENU_DRINK order by Name_DRINK; ";
            daMenuDrink = new SqlDataAdapter(sql_drink, conn);
            dtMenuDrink = new DataTable();
            daMenuDrink.Fill(dtMenuDrink);

            cbbMenu_Drink.DataSource = dtMenuDrink;
            cbbMenu_Drink.DisplayMember = "Tên món";
            cbbMenu_Drink.ValueMember = "Mã món";

            //Lấy dữ liệu trong bảng Order food  
            string sql_order_food = "select ID_BILL as N'Mã Hóa đơn',ID_Food as N'Mã món', NOF as N'Số lượng'  from ORDER_FOOD where ID_Bill = '"+lbID_Bill+"'";
            daOrderFood = new SqlDataAdapter(sql_order_food, conn);
            dtOrderFood = new DataTable();
            daOrderFood.Fill(dtOrderFood);
            
            //Lấy dữ liệu trong bảng Order Drink
            string sql_order_drink = "select ID_BILL as N'Mã Hóa đơn',ID_Drink as N'Mã món', NOD as N'Số lượng'  from ORDER_DRINK where ID_Bill = '" + lbID_Bill + "'";
            daOrderDrink = new SqlDataAdapter(sql_order_drink, conn);
            dtOrderDrink = new DataTable();
            daOrderDrink.Fill(dtOrderDrink);
            if (dtOrderFood.Rows.Count > 0 || dtOrderDrink.Rows.Count >0) //Nếu mà bàn đã có hóa đơn  thì ẩn nút tạo hóa đơn mới
            {
                panel1.Visible = false;
            }


            //Load lấy dữ liệu trong bảng thống kê
            string sql_sttc = "select ID_STTC as N'Mã thống kê', Total_Revenue as N'Tổng thu', Total_Cost as N'Tổng chi' , Total_Profit as N'Lợi nhuận' , Times as N'Ngày thống kê' from STATISTICAL order by Times; ";
            daStatistical = new SqlDataAdapter(sql_sttc, conn);
            dtStatistical = new DataTable();
            daStatistical.Fill(dtStatistical);

            //

            if (dtOrderFood.Columns.Contains("Tên món") == false)
            {
                dtOrderFood = new DataTable();
                dtOrderFood.Columns.Add("Tên món");
                dtOrderFood.Columns.Add("Số lượng");
                dtOrderFood.Columns.Add("Giá tiền");
                dtOrderFood.Columns.Add("Thành tiền");
            }
            if (dtOrderDrink.Columns.Contains("Tên món") == false)
            {
                dtOrderDrink = new DataTable();
                dtOrderDrink.Columns.Add("Tên món");
                dtOrderDrink.Columns.Add("Số lượng");
                dtOrderDrink.Columns.Add("Giá tiền");
                dtOrderDrink.Columns.Add("Thành tiền");
            }
            this.refreshCBB();

            
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        //Nút gọi món -- Đã xong
        private void Order_food_Click(object sender, EventArgs e)
        {
            if (numericUpDown1.Value != 0) //Nếu số lượng khác 0 thì thực hiện gọi món
            {
                //Thêm dữ liệu vào sql
                try
                {
                    string sql_insert_order = "insert into ORDER_FOOD values('" + lbID_Bill.Text + "','" + cbbMenu_Food.SelectedValue.ToString() + "'," + numericUpDown1.Value.ToString() + ")";
                    SqlCommand cmdtry = new SqlCommand(sql_insert_order, conn);
                    cmdtry.ExecuteNonQuery();

                }
                catch //xử lý nếu món bị trùng thì tăng số lượng
                {
                    string sql_update_NOF = "update ORDER_FOOD set NOF += " + numericUpDown1.Value.ToString() + " where ID_Bill = '" + lbID_Bill.Text + "' and ID_Food ='" + cbbMenu_Food.SelectedValue.ToString() + "'";
                    SqlCommand cmdcatch = new SqlCommand(sql_update_NOF, conn);
                    cmdcatch.ExecuteNonQuery();
                }

                //Lấy ra giá tiền của món ăn đó
                string select_Price = "Select Price_Food from MENU_FOOD where ID_Food = '" + cbbMenu_Food.SelectedValue.ToString() + "';";
                daGetPrice = new SqlDataAdapter(select_Price, conn);
                dtGetPrice = new DataTable();
                daGetPrice.Fill(dtGetPrice);
                dgvOrderFood.DataSource = dtOrderFood;


                bool checkInOrder = false; 
                for (int i = 0; i < dtOrderFood.Rows.Count; i++) //Kiểm tra data table orderFood xem đã có món ăn định thêm hay chưa
                {
                    if (dtOrderFood.Rows[i][0].ToString() == cbbMenu_Food.Text) //Nếu mà gặp dòng đã lưu tên món thì
                    {
                        dtOrderFood.Rows[i][1] = Convert.ToInt32(dtOrderFood.Rows[i][1]) + numericUpDown1.Value; //Tăng giá trị của số món đang chọn lên n 
                        dtOrderFood.Rows[i][3] = Convert.ToInt32(dtGetPrice.Rows[0][0]) * Convert.ToInt32(dtOrderFood.Rows[i][1]); //Tính lại tổng tiền của món đó dựa trên số lượng mới
                        checkInOrder = true; //Đánh dấu có và thoát khỏi vòng lặp kiểm tra
                        break;
                    }
                }
                if (!checkInOrder) //Nếu mà kiểm tra hết data table order food mà không có tên món đã chọn trùng với món đang định thêm thì thêm vào như bình thường
                {
                    dtOrderFood.Rows.Add(cbbMenu_Food.Text, numericUpDown1.Value.ToString(), Convert.ToInt32(dtGetPrice.Rows[0][0]), Convert.ToInt32(dtGetPrice.Rows[0][0]) * numericUpDown1.Value);
                }
            }
            else
            {
                MessageBox.Show("Để gọi món thì số lượng phải lớn hơn 0 !");
                numericUpDown1.Focus();
            }
            this.refreshCBB();



        }

        private void dgvOrderFood_CellClick(object sender, DataGridViewCellEventArgs e)
        {
             indexFood = e.RowIndex;
            try
            {
                cbbMenu_Food.Text = dgvOrderFood.Rows[indexFood].Cells[0].Value.ToString();
                numericUpDown1.Value = Convert.ToInt32(dgvOrderFood.Rows[indexFood].Cells[1].Value);
            }
            catch
            {

            }
        }

        private void dgvOrderDrink_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            indexDrink = e.RowIndex;
            try
            {
                cbbMenu_Drink.Text = dgvOrderDrink.Rows[indexDrink].Cells[0].Value.ToString();
                numericUpDown2.Value = Convert.ToInt32(dgvOrderDrink.Rows[indexDrink].Cells[1].Value);
            }
            catch
            {

            }
        }
        //Xóa món ăn
        private void Delete_food_Click(object sender, EventArgs e)
        {
            try
            {

                dtOrderFood.Rows[indexFood].Delete();
                dgvOrderFood.DataSource = dtOrderFood;
                string sql_deleteFood = "Delete ORDER_FOOD where ID_BIll ='" + lbID_Bill.Text + "' and ID_Food='" + cbbMenu_Food.SelectedValue.ToString() + "'";
                SqlCommand cmd_delete1 = new SqlCommand(sql_deleteFood, conn);
                cmd_delete1.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("Lỗi");
            }
        }
        //Cập nhật đồ ăn
        private void Update_food_Click(object sender, EventArgs e)
        {
            // Chỉ Thay đổi số lượng
            try
            {
                if (numericUpDown1.Value > 0)//Nếu số lượng lớn hơn 0 thì thay đổi
                {
                    string sql_update_order_food = "update ORDER_FOOD set NOF = " + numericUpDown1.Value.ToString() + "where ID_Bill='" + lbID_Bill.Text + "' and ID_Food ='" + cbbMenu_Food.SelectedValue.ToString() + "'";
                    SqlCommand cmd = new SqlCommand(sql_update_order_food, conn);
                    cmd.ExecuteNonQuery();
                    dtOrderFood.Rows[indexFood][1] = numericUpDown1.Value;
                    dtOrderFood.Rows[indexFood][3] = Convert.ToInt32(dtOrderFood.Rows[indexFood][1]) * Convert.ToInt32(dtOrderFood.Rows[indexFood][2]);
                }
                else
                {
                    MessageBox.Show("Số lượng phải lơn hơn 0!");
                }
            }
            catch
            {
                MessageBox.Show("Lỗi");
            }
            
        }

        private void Update_drink_Click(object sender, EventArgs e)
        {
            // Chỉ Thay đổi số lượng
            try
            {   if(numericUpDown2.Value > 0)//Nếu số lượng lớn hơn 0 thì thay đổi
                {
                    string sql_update_order_drink = "update ORDER_DRINK set NOD = " + numericUpDown2.Value.ToString() + "where ID_Bill='" + lbID_Bill.Text + "' and ID_Drink ='" + cbbMenu_Drink.SelectedValue.ToString() + "'";
                    SqlCommand cmd = new SqlCommand(sql_update_order_drink, conn);
                    cmd.ExecuteNonQuery();
                    dtOrderDrink.Rows[indexDrink][1] = numericUpDown2.Value;
                    dtOrderDrink.Rows[indexDrink][3] = Convert.ToInt32(dtOrderDrink.Rows[indexDrink][1]) * Convert.ToInt32(dtOrderDrink.Rows[indexDrink][2]);
                }
                else
                {
                    MessageBox.Show("Số lượng phải lơn hơn 0!");
                }


            }
            catch
            {
                MessageBox.Show("Lỗi");
            }

        }


        //Xóa đồ uống
        private void Delete_drink_Click(object sender, EventArgs e)
        {
            try
            {
                dtOrderDrink.Rows[indexDrink].Delete();
                dgvOrderDrink.DataSource = dtOrderDrink;
                string sql_deleteDrink = "Delete ORDER_Drink where ID_BIll ='" + lbID_Bill.Text + "' and ID_Drink='" + cbbMenu_Drink.SelectedValue.ToString() + "'";
                SqlCommand cmd_delete2 = new SqlCommand(sql_deleteDrink, conn);
                cmd_delete2.ExecuteNonQuery();
                this.refreshCBB();
            }
            catch
            {
                MessageBox.Show("Lỗi");
            }
        }

 

        //Nút gọi đồ uống -đã xong
        private void Order_drink_Click(object sender, EventArgs e)
        {
            if (numericUpDown2.Value != 0)
            {
                //Thêm dữ liệu vào sql
                try
                {
                    string sql_insert_drink = "insert into ORDER_DRINK values('" + lbID_Bill.Text + "','" + cbbMenu_Drink.SelectedValue.ToString() + "'," + numericUpDown2.Value.ToString() + ")";
                    SqlCommand cmdtry = new SqlCommand(sql_insert_drink, conn);
                    cmdtry.ExecuteNonQuery();

                }
                catch //xử lý nếu món bị trùng thì tăng số lượng
                {
                    string sql_update_NOD = "update ORDER_DRINK set NOD += " + numericUpDown2.Value.ToString() + " where ID_Bill = '" + lbID_Bill.Text + "' and ID_Drink ='" + cbbMenu_Drink.SelectedValue.ToString() + "'";
                    SqlCommand cmdcatch = new SqlCommand(sql_update_NOD, conn);
                    cmdcatch.ExecuteNonQuery();
                }
                //Lấy ra giá tiền của món ăn đó
                string select_Price = "Select Price_Drink from MENU_DRINK where ID_Drink = '" + cbbMenu_Drink.SelectedValue.ToString() + "';";
                daGetPrice = new SqlDataAdapter(select_Price, conn);
                dtGetPrice = new DataTable();
                daGetPrice.Fill(dtGetPrice);
                dgvOrderDrink.DataSource = dtOrderDrink;
                bool checkInOrder = false;
                for (int i = 0; i < dtOrderDrink.Rows.Count; i++)
                {
                    if (dtOrderDrink.Rows[i][0].ToString() == cbbMenu_Drink.Text) //Nếu mà trùng món đã có trong danh sách order
                    {
                        dtOrderDrink.Rows[i][1] = Convert.ToInt32(dtOrderDrink.Rows[i][1]) + numericUpDown2.Value;
                        dtOrderDrink.Rows[i][3] = Convert.ToInt32(dtGetPrice.Rows[0][0]) * Convert.ToInt32(dtOrderDrink.Rows[i][1]);
                        checkInOrder = true;
                        break;
                    }
                }
                if (!checkInOrder)
                {
                    dtOrderDrink.Rows.Add(cbbMenu_Drink.Text, numericUpDown2.Value.ToString(), Convert.ToInt32(dtGetPrice.Rows[0][0]), Convert.ToInt32(dtGetPrice.Rows[0][0]) * numericUpDown2.Value);
                }
            }
            else
            {
                MessageBox.Show("Để gọi món thì số lượng phải lớn hơn 0 !");
                numericUpDown2.Focus();
            }
            this.refreshCBB();
        }
        //Nút tạo hóa đơn mới nếu như bàn này trống -- đã xong
        private void button1_Click(object sender, EventArgs e)
        {
            
            string ID_STTC;
            try
            {   //Lấy ra mã thống kê
                string sql_select_id_sttc = "select ID_STTC from STATISTICAL where times = '" + DateTime.Now.ToShortDateString() + "'";
                daGetIdSTTC = new SqlDataAdapter(sql_select_id_sttc, conn);
                dtGetIdSTTC = new DataTable();
                daGetIdSTTC.Fill(dtGetIdSTTC);
                 ID_STTC = dtGetIdSTTC.Rows[0][0].ToString();

            }
            catch // Nếu mà chưa có dữ liệu thống kê ngày hôm nay thì thêm hóa đơn rồi mới lấy mã thống kê
            {
                ID_STTC = "S_" + Convert.ToString(random.Next(1000000)); //Tạo ra ID mới để không bị trùng
                string cmd_Insert_STTC = "insert into STATISTICAL values('" + ID_STTC + "',0,0,0,'"+DateTime.Now.ToShortDateString() +"')";
                SqlCommand cmd1 = new SqlCommand(cmd_Insert_STTC, conn);
                cmd1.ExecuteNonQuery();
                dtStatistical.Rows.Clear();
                daStatistical.Fill(dtStatistical);

                
            }
            

            //Thêm dữ liệu mới vào bảng BIll với ID_STTC tham chiếu vừa tìm được
            string ID_Bill = "B_" + Convert.ToString(random.Next(1000000));
            string cmd_Insert = "insert into BILL values('" + ID_Bill + "',N'"  +this.Text + "',0,'" + DateTime.Now.ToString() + "','" + ID_STTC+ "')";
            SqlCommand cmd = new SqlCommand(cmd_Insert, conn);
            try
            {
                cmd.ExecuteNonQuery();
                
            }
            catch
            {
                MessageBox.Show("Dữ liệu thêm vào bị trùng !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            panel1.Visible = false;
            lbID_Bill.Text = ID_Bill;
        }



        //Nút thoát phần quản lý bàn --đã xong
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       
        private void GoiMon_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(checkPay==false && (dgvOrderDrink.Rows.Count >0 || dgvOrderFood.Rows.Count >0 ) )//Nếu chưa thanh toán thì sẽ để màu đỏ
            {
               Form1.instance.changeColor.BackColor = Color.Red;

            }
           
        }

      

        //Nút thanh toán sẽ cập nhật dữ liệu vào sql - đã xong
        private void button3_Click(object sender, EventArgs e)
        {
            int total_bill = 0;
            //Cập nhật tổng hóa đơn
            for(int i = 0; i < dgvOrderFood.Rows.Count; i++)
            {
                total_bill += Convert.ToInt32(dgvOrderFood.Rows[i].Cells[3].Value);
            }
            for (int i = 0; i < dgvOrderDrink.Rows.Count; i++)
            {
                total_bill += Convert.ToInt32(dgvOrderDrink.Rows[i].Cells[3].Value);
            }
            textBox2.Text=total_bill.ToString();

            string cmd_update_total_bill = "update BILL set Total_Bill = "+total_bill.ToString() + "where ID_Bill='" + lbID_Bill.Text + "'";
            SqlCommand cmd1 = new SqlCommand(cmd_update_total_bill,conn);
            cmd1.ExecuteNonQuery();
            
            string cmd_update_STTC1 = "update STATISTICAL set Total_Revenue = (select sum(Total_Bill) from BILL where ID_STTC = (select ID_STTC from STATISTICAL where Times = '"+DateTime.Now.ToShortDateString()+ "')), Total_Cost = (select sum(Total_ex) from EXSPENDABLES where Times ='"+DateTime.Now.ToShortDateString()+ "'), Total_Profit = (Total_Revenue - Total_Cost) where ID_STTC= (select ID_STTC from STATISTICAL where Times ='"+DateTime.Now.ToShortDateString()+"')";
            SqlCommand cmd2 = new SqlCommand(cmd_update_STTC1, conn);
            cmd2.ExecuteNonQuery();
            string cmd_update_STTC2 = "update STATISTICAL set Total_Profit = (Total_Revenue - Total_Cost) where Times ='" + DateTime.Now.ToShortDateString() + "'";
            SqlCommand cmd3 = new SqlCommand(cmd_update_STTC2, conn);
            cmd2.ExecuteNonQuery();

            MessageBox.Show("Tổng tiền phải trả "+total_bill.ToString());
            Form1.instance.changeColor.BackColor = Color.Transparent;
            checkPay = true;

        }
    }
}
