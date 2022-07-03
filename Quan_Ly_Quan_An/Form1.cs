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

    public partial class Form1 : Form
    {
        public static Form1 instance;
        public Button changeColor;
        public Form1()
        {
            InitializeComponent();
            instance = this;
            changeColor = new Button();
        }
        int index;
        string ToDay;
        Random random = new Random();
        DataTable dtMenuFood , dtMenuDrink, dtExspendables,dtStatistical;
        SqlDataAdapter daMenuFood = new SqlDataAdapter();
         SqlDataAdapter daMenuDrink = new SqlDataAdapter();
         SqlDataAdapter daBill = new SqlDataAdapter();
         SqlDataAdapter daOrderFood = new SqlDataAdapter();
         SqlDataAdapter daOrderDrink = new SqlDataAdapter();
         SqlDataAdapter daExspendables = new SqlDataAdapter();
         SqlDataAdapter daStatistical = new SqlDataAdapter();
        string chuoiketnoi = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=QLQuanAn;Integrated Security=True";
        SqlConnection conn = null;

        public static  GoiMon[] ArrayGoiMon = new GoiMon[24]; //Tạo ra mảng lưu 24 form của từng bàn

        //Load form 
        private void Form1_Load(object sender, EventArgs e)
        {
            //Tạo kết nối
            conn = new SqlConnection(chuoiketnoi);
            conn.Open();

            //Load tab đồ ăn
            string sql_food = "select ID_Food as N'Mã món', Name_Food as N'Tên món', Price_Food as N'Giá tiền' , Describe as N'Mô tả' from MENU_FOOD order by Name_Food; ";
            daMenuFood = new SqlDataAdapter(sql_food, conn);
            dtMenuFood = new DataTable();
            daMenuFood.Fill(dtMenuFood);
            dgvMenuFood.DataSource = dtMenuFood;

            //Load tab đồ uống
            string sql_drink = "select ID_Drink as N'Mã món', Name_Drink as N'Tên món', Price_Drink as N'Giá tiền' , Describe_Drink as N'Mô tả' from MENU_DRINK order by Name_DRINK; ";
            daMenuDrink = new SqlDataAdapter(sql_drink, conn);
            dtMenuDrink = new DataTable();
            daMenuDrink.Fill(dtMenuDrink);
            dgvMenuDrink.DataSource = dtMenuDrink;

            //Load tab Quản lý nguyên Liệu
            ToDay = DateTime.Now.ToShortDateString().ToString();
            lbTime.Text = "Ngày : " + ToDay;
            string sql_Exspend = "select ID_Exspend as N'Mã nguyên liệu', Name_material as N'Tên Nguyên liệu', Quantity as N'Số lượng' , Price_material as N'Giá tiền',Total_ex as N'Thành tiền' , Times as N'Ngày mua',ID_STTC as N'Mã thống kê' from EXSPENDABLES order by Times; ";
            daExspendables = new SqlDataAdapter(sql_Exspend, conn);
            dtExspendables = new DataTable();
            daExspendables.Fill(dtExspendables);
            dgvExspendables.DataSource = dtExspendables;
       


            //Load tab Thống kê

            string sql_sttc = "select ID_STTC as N'Mã thống kê', Total_Revenue as N'Tổng thu', Total_Cost as N'Tổng chi' , Total_Profit as N'Lợi nhuận' , Times as N'Ngày thống kê' from STATISTICAL order by Times; ";
            daStatistical = new SqlDataAdapter(sql_sttc, conn);
            dtStatistical = new DataTable();
            daStatistical.Fill(dtStatistical);
            dgvStatistical.DataSource = dtStatistical;

            for(int i = 0; i < 24; i++)
            {
                ArrayGoiMon[i] = new GoiMon();
            }
            //Load datagridview thống kê món ăn được yêu thích
            SqlDataAdapter daFavoriteFood = new SqlDataAdapter("select MENU_FOOD.Name_Food as N'Tên món ăn', sum(NOF) as N'Số lần được gọi' from ORDER_FOOD,MENU_FOOD where (ORDER_FOOD.ID_Food = MENU_FOOD.ID_Food) group by MENU_FOOD.Name_Food", conn);
            DataTable dtFavoriteFood = new DataTable();
            daFavoriteFood.Fill(dtFavoriteFood);
            dgvFavoriteFood.DataSource = dtFavoriteFood;

            //Load datagirdview Thống kê đồ uống được yêu thích
            SqlDataAdapter daFavoriteDrink = new SqlDataAdapter("select MENU_DRINK.Name_Drink as N'Tên món ăn', sum(NOD) as N'Số lần được gọi' from ORDER_Drink,MENU_Drink where (ORDER_Drink.ID_Drink = MENU_Drink.ID_Drink) group by MENU_Drink.Name_Drink", conn);
            DataTable dtFavoriteDrink = new DataTable();
            daFavoriteDrink.Fill(dtFavoriteDrink);
            dgvFavoriteDrink.DataSource = dtFavoriteDrink;









        }



        // = = = = = = = = = = = = = = = Xử lý thông tin tại MENU đồ ăn = = = = = = = = = = = = = = 

        //Lấy ra chỉ số và đổ vào các ô phía trên
        private void dgvMenuFood_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            try {
                tbID_Food.Text = dgvMenuFood.Rows[index].Cells[0].Value.ToString();
                tbName_Food.Text = dgvMenuFood.Rows[index].Cells[1].Value.ToString();
                tbPrice_Food.Text = dgvMenuFood.Rows[index].Cells[2].Value.ToString();
                tbDescribe.Text = dgvMenuFood.Rows[index].Cells[3].Value.ToString();
            }
            catch {
               
            }
        }
        //Nút thêm dữ liệu vào MENU đồ ăn
        private void btnAdd_menu_food_Click(object sender, EventArgs e)
        {
            string ID_Food = "F_" + Convert.ToString(random.Next(1000000)); //Tạo ID tự động
            tbID_Food.Text = ID_Food;
            string cmd_Insert = "insert into MENU_FOOD values('" + tbID_Food.Text + "'," + "N'" + tbName_Food.Text + "'," 
                + Convert.ToInt32(tbPrice_Food.Text) + ",N'" + tbDescribe.Text + "')";
            SqlCommand cmd = new SqlCommand(cmd_Insert, conn);
            try
            {
                cmd.ExecuteNonQuery();
                dtMenuFood.Rows.Clear();
                daMenuFood.Fill(dtMenuFood);
                dgvMenuFood.DataSource = dtMenuFood;
            }
            catch //Xuất thông báo nếu mã đã bị trùng
            {
                MessageBox.Show("Dữ liệu thêm vào bị trùng !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbName_Food.Focus();

            }

        }

        //Nút sửa dữ liệu bảng MENU món ăn
        private void btnUpdate_food_Click(object sender, EventArgs e)
        {
            string cmd_Insert = "update MENU_FOOD set Name_Food = N'" + tbName_Food.Text + "',Price_Food = " +
                Convert.ToInt32(tbPrice_Food.Text) + ",Describe = N'" + tbDescribe.Text + "' where ID_Food = '" + tbID_Food.Text + "'";
            SqlCommand cmd = new SqlCommand(cmd_Insert, conn);
            cmd.ExecuteNonQuery();
            dtMenuFood.Rows.Clear();
            daMenuFood.Fill(dtMenuFood);
            dgvMenuFood.DataSource = dtMenuFood;
            //clear input
            tbID_Food.Text = "";
            tbName_Food.Text = "";
            tbPrice_Food.Text = "";
            tbDescribe.Text = "";
        }

        //Nút xóa dữ liệu bảng MENU món ăn

        private void btnDelete_food_Click(object sender, EventArgs e)
        {
            string sql_delete = "delete MENU_FOOD where ID_Food = '" + tbID_Food.Text + "';";
            SqlCommand cmd = new SqlCommand(sql_delete, conn);
            try
            {
                cmd.ExecuteNonQuery();
                dtMenuFood.Rows.Clear();
                dtMenuFood = new DataTable();
                daMenuFood.Fill(dtMenuFood);
                dgvMenuFood.DataSource = dtMenuFood;
                //clear input
                tbID_Food.Text = "";
                tbName_Food.Text = "";
                tbPrice_Food.Text = "";
                tbDescribe.Text = "";
            }
            catch
            {
                MessageBox.Show("Lỗi");
            }


        }
       
    

        // = = = = = = = = = = = = = = = Xử lý thông tin tại MENU đồ uống = = = = = = = = = = = = = = 

        //Thêm dữ liệu vào bảng đồ uống
       
        private void btnAdd_menu_drink_Click(object sender, EventArgs e)
        {
            string ID_Drink = "D_" + Convert.ToString(random.Next(1000000));//Tạo mã tự động

            string cmd_Insert = "insert into MENU_DRINK values('" + ID_Drink + "'," + "N'" + tbName_Drink.Text + "'," + Convert.ToInt32(tbPrice_Drink.Text) + ",N'" + tbDescribe_Drink.Text + "')";
            SqlCommand cmd = new SqlCommand(cmd_Insert, conn);
            try
            {
                cmd.ExecuteNonQuery();
                dtMenuDrink.Rows.Clear();
                daMenuDrink.Fill(dtMenuDrink);
                dgvMenuDrink.DataSource = dtMenuDrink;
            }
            catch//xuất thông báo nếu mã đã bị trùng
            {
                MessageBox.Show("Dữ liệu thêm vào bị trùng !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbName_Drink.Focus();

            }
        }



        //Xóa dữ liệu trong bảng đồ uống
        
        private void btnDelete_drink_Click(object sender, EventArgs e)
        {
            string sql_delete = "delete MENU_DRINK where ID_Drink = '" + tbID_Drink.Text + "';";
            SqlCommand cmd = new SqlCommand(sql_delete, conn);
            try
            {
                cmd.ExecuteNonQuery();
                dtMenuDrink.Rows.Clear();
                dtMenuDrink = new DataTable();
                daMenuDrink.Fill(dtMenuDrink);
                dgvMenuDrink.DataSource = dtMenuDrink;
                //clear input
                tbID_Drink.Text = "";
                tbName_Drink.Text = "";
                tbPrice_Drink.Text = "";
                tbDescribe_Drink.Text = "";
            }
            catch
            {
                MessageBox.Show("Lỗi");
            }
        }



        //Cập nhật dữ liệu trong bảng đồ uống
        private void btnUpdate_drink_Click(object sender, EventArgs e)
        {
            string cmd_update = "update MENU_DRINK set Name_DRINK = N'" + tbName_Drink.Text + "',Price_Drink = " + Convert.ToInt32(tbPrice_Drink.Text) + ",Describe_Drink = N'" + tbDescribe_Drink.Text + "' where ID_Drink = '" + tbID_Drink.Text + "'";
            SqlCommand cmd = new SqlCommand(cmd_update, conn);
            cmd.ExecuteNonQuery();
            dtMenuDrink.Rows.Clear();
            daMenuDrink.Fill(dtMenuDrink);
            dgvMenuDrink.DataSource = dtMenuDrink;
            //clear input
            tbID_Drink.Text = "";
            tbName_Drink.Text = "";
            tbPrice_Drink.Text = "";
            tbDescribe_Drink.Text = "";
        }



        //Hiển thị dữ liệu lên các ô phía trên
        private void dgvMenuDrink_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            try
            {
                tbID_Drink.Text = dgvMenuDrink.Rows[index].Cells[0].Value.ToString();
                tbName_Drink.Text = dgvMenuDrink.Rows[index].Cells[1].Value.ToString();
                tbPrice_Drink.Text = dgvMenuDrink.Rows[index].Cells[2].Value.ToString();
                tbDescribe_Drink.Text = dgvMenuDrink.Rows[index].Cells[3].Value.ToString();
            }
            catch
            {
          
            }
        }

        

        // = = = = == = = == = == = = = Xử lý thông tin tại Tab quản lý Nguyên Liệu = = = = = = = //

        private void UPDATE_STATISTICAL(string time) // Cập nhật lại tổng tiền đã chi tiêu và thu nhập vào bảng thống kê theo thời gian
        {
            int total_re,total_ex;
            string TongChi = "select sum(Total_ex) from EXSPENDABLES where Times='"+time+"'"; 
            SqlDataAdapter daTongChi = new SqlDataAdapter(TongChi, conn);
            DataTable dtTongChi = new DataTable();
            daTongChi.Fill(dtTongChi);
            try
            {
                total_ex = Convert.ToInt32(dtTongChi.Rows[0][0]); //Lấy ra tổng số tiền đã chi trong ngày
            }
            catch
            {
                total_ex = 0; //Nếu như chưa có dữ liệu thì tổng tiền chi sẽ bị NULL gây ra lỗi
            }
            string TongThu = "select sum(Total_Bill) from BILL where ID_STTC = (select ID_STTC from STATISTICAL where Times = '" + time + "');";
            SqlDataAdapter daTongThu = new SqlDataAdapter(TongThu,conn);
            DataTable dtTongThu = new DataTable();
            daTongThu.Fill(dtTongThu);
            try
            {
                total_re = Convert.ToInt32(dtTongThu.Rows[0][0]); //Lấy ra tổng số tiền đã thu được trong ngày
            }
            catch
            {
                total_re = 0;//Nếu như chưa có dữ liệu thì tổng tiền thu sẽ bị NULL gây ra lỗi
            }
            string cmd_update_STTC = "update STATISTICAL set Total_Cost = " +total_ex.ToString() +", Total_Revenue = " + total_re.ToString() +", Total_Profit = "+(total_re - total_ex).ToString()+ "Where Times = '"+ time+"' "; //Thực hiện câu lệnh thay đổi tổng 
            SqlCommand cmd1 = new SqlCommand(cmd_update_STTC, conn);
            cmd1.ExecuteNonQuery();
            dtStatistical.Rows.Clear();
            daStatistical.Fill(dtStatistical);
            dgvStatistical.DataSource = dtStatistical;
        }

        int price = 0;

        //Xử lý ngoại lệ tại ô nhập giá
        private void tbPrice_ex_Leave(object sender, EventArgs e)
        {
            try
            {
                price = Convert.ToInt32(tbPrice_ex.Text);
                if(price <= 0) //nếu nhập vào giá nhỏ nhơn hoặc bằng 0
                {
                    MessageBox.Show("Nhập giá tiền là số nguyên dương lớn hơn 0");
                    tbPrice_ex.Focus();
                }
                tbTotal_price.Text = Convert.ToString(price * numericUpDown1.Value);
            }
            catch {
                MessageBox.Show("Nhập giá tiền là số nguyên dương !");
                tbPrice_ex.Focus();
            }
        }


        //Xử lý tính giá tiền của một nguyên liệu
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                tbTotal_price.Text = Convert.ToString(numericUpDown1.Value * Convert.ToInt32(tbPrice_ex.Text));
            }
            catch
            {
               
            }
        }

        

        //Thêm dữ liệu vào bảng Nguyên Liệu
        private void btnAdd_ex_Click(object sender, EventArgs e)
        {
           
            bool Exits = false;
            string sql_check = "select count(*) from STATISTICAL where Times='" + ToDay + "'"; // Kiểm tra xem trong dữ liệu trong bảng thống kê đã có ngày hôm nay chưa
            SqlDataAdapter dacheckExits = new SqlDataAdapter(sql_check, conn);
            DataTable check = new DataTable();
            dacheckExits.Fill(check);
            if (check.Rows[0][0].ToString() == "1")
            {
                Exits = true;
            }
            if (Exits == true) // nếu mà đã có trong bảng thống kê thực hiện thêm dữ liệu như bình thường
            {
                string ID_Exspend = "E_" + Convert.ToString(random.Next(1000000));
                string cmd_Insert = "insert into EXSPENDABLES values('" + ID_Exspend + "'," + "N'" + tbName_Material.Text + "'," + numericUpDown1.Value.ToString()
                    + ", " + tbPrice_ex.Text + "," + tbTotal_price.Text + ",'" + ToDay + "', (select ID_STTC from STATISTICAL where Times='" +ToDay + "'))";
                try
                {
                    SqlCommand cmd = new SqlCommand(cmd_Insert, conn);
                    cmd.ExecuteNonQuery();
                    dtExspendables.Rows.Clear();
                    daExspendables.Fill(dtExspendables);
                    dgvExspendables.DataSource = dtExspendables;
                    UPDATE_STATISTICAL(ToDay);
                }
                catch //Đề phòng trường hợp mã đã bị trùng 
                {
                    MessageBox.Show("Dữ liệu thêm vào bị trùng !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbName_Material.Focus();
                }
               
            }
            else //nếu mà không có thì tạo mới một dữ liệu ở bảng thống kê và thêm như bình thường
            {
                //Thêm một hóa đơn vào bảng thống kê
                string ID_STTC = "S_" + Convert.ToString(random.Next(1000000)); //Tạo ra ID mới để không bị trùng
                string cmd_Insert_STTC = "insert into STATISTICAL values('" + ID_STTC + "',0,0,0,'" + ToDay + "');";
                SqlCommand cmd1 = new SqlCommand(cmd_Insert_STTC, conn);
                cmd1.ExecuteNonQuery();
                dtStatistical.Rows.Clear();
                daStatistical.Fill(dtStatistical);
                dgvStatistical.DataSource = dtStatistical;
                //Thêm dữ liệu vào bảng chi tiêu
                string ID_Exspend = "E_" + Convert.ToString(random.Next(1000000));
                string cmd_Insert_EX = "insert into EXSPENDABLES values('" + ID_Exspend + "'," + "N'" + tbName_Material.Text + "'," + numericUpDown1.Value.ToString()
                    + ", " + tbPrice_ex.Text + "," + tbTotal_price.Text + ",'" + ToDay + "','" + ID_STTC + "')";
                SqlCommand cmd2 = new SqlCommand(cmd_Insert_EX, conn);
                try
                {
                     cmd2.ExecuteNonQuery();
                    dtExspendables.Rows.Clear();
                    daExspendables.Fill(dtExspendables);
                    dgvExspendables.DataSource = dtExspendables;
                    UPDATE_STATISTICAL(ToDay); 
                }
                catch //Phòng trường hợp mã bị trùng
                {
                    MessageBox.Show("Dữ liệu thêm vào bị trùng !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbName_Material.Focus();
                }
                        
            }
        }
       


        //Hiển thị dữ liệu tại ô đã chọn lên trên
        private void dgvExspendables_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            try
            {
                tbID_Ex.Text = dgvExspendables.Rows[index].Cells[0].Value.ToString();
                tbName_Material.Text = dgvExspendables.Rows[index].Cells[1].Value.ToString();
                tbPrice_ex.Text = dgvExspendables.Rows[index].Cells[3].Value.ToString();
                numericUpDown1.Value = Convert.ToInt32(dgvExspendables.Rows[index].Cells[2].Value);
                tbTotal_price.Text = dgvExspendables.Rows[index].Cells[4].Value.ToString();
                lbTime.Text =((DateTime)dgvExspendables.Rows[index].Cells[5].Value).ToShortDateString();
            }
            catch
            {

            }
            
            
        }
        //cập nhật dữ liệu trong bảng Chi tiêu
        private void Update_Exspendables_Click(object sender, EventArgs e)
        {
            string cmd_update = "update EXSPENDABLES set Name_material = N'" + tbName_Material.Text + "',Quantity = " + Convert.ToString(numericUpDown1.Value) + ",Price_material = " + tbPrice_ex.Text + ",Total_ex = "+tbTotal_price.Text+" where ID_Exspend = '" + tbID_Ex.Text + "'";
            SqlCommand cmd = new SqlCommand(cmd_update, conn);
            cmd.ExecuteNonQuery();
            dtExspendables.Rows.Clear();
            daExspendables.Fill(dtExspendables);
            dgvExspendables.DataSource = dtExspendables;
            //clear input
            tbID_Ex.Text = "";
            tbName_Material.Text = "";
            tbPrice_ex.Text = "";
            tbTotal_price.Text = "";
            //Cập nhật lại bảng thống kê
            UPDATE_STATISTICAL(lbTime.Text);

        }
        //Xóa dữ liệu tại bảng Chi tiêu
        private void Delete_Material_Click(object sender, EventArgs e)
        {
            string sql_delete = "delete EXSPENDABLES where ID_Exspend = '" + tbID_Ex.Text + "';";
            SqlCommand cmd = new SqlCommand(sql_delete, conn);
            try
            {  
                cmd.ExecuteNonQuery();
                dtExspendables.Rows.Clear();
                dtExspendables = new DataTable();
                daExspendables.Fill(dtExspendables);
                dgvExspendables.DataSource = dtExspendables;
                //clear input
                tbID_Ex.Text = "";
                tbName_Material.Text = "";
                tbPrice_ex.Text = "";
                tbTotal_price.Text = "";

                //Cập nhật lại bảng thống kê
                UPDATE_STATISTICAL(lbTime.Text);
                
            }
            catch
            {
                MessageBox.Show("Lỗi");
            }
        }


        // = = = = == = = == = == = = = Xử lý thông tin tại Tab quản lý Bàn = = = = = = = //
      
        private void btnQuan_ly_ban_Click(object sender, EventArgs e)
        {
            if (((Control)sender).BackColor == Color.Red) // Nếu mà bàn đang có khách ngồi thì mở cửa sổ ra
            {

                index = Convert.ToInt32(((Control)sender).Text) - 1;
                changeColor = (Button)sender;
                ArrayGoiMon[index].ShowDialog();



            }
            else // Nếu bàn đang trống - chưa có khách ngồi
            {
                //Tạo một bảng mới để lưu

                index = Convert.ToInt32(((Control)sender).Text) - 1;
                ArrayGoiMon[index] = new GoiMon();
                ArrayGoiMon[index].Text = "Bàn số " + ((Control)sender).Text;
                changeColor = (Button)sender;
                ArrayGoiMon[index].ShowDialog();
            }

        }



        // = = = == = = = = Xử lý thông tin tại tab Thống kê = = = = = = //
        private void start_date_ValueChanged(object sender, EventArgs e)
        {
            //Dữ liệu trong khoảng thời gian
            string sql_sttc = "select ID_STTC as N'Mã thống kê', Total_Revenue as N'Tổng thu', Total_Cost as N'Tổng chi' , Total_Profit as N'Lợi nhuận' , Times as N'Ngày thống kê' from STATISTICAL where Times Between '"+start_date.Value.ToShortDateString()+"' And '"+end_date.Value.ToShortDateString()+"'";
            daStatistical = new SqlDataAdapter(sql_sttc, conn);
            dtStatistical = new DataTable();
            daStatistical.Fill(dtStatistical);
            dgvStatistical.DataSource = dtStatistical;
        }
        private void cbbChonThang_SelectedIndexChanged(object sender, EventArgs e)//Chọn tháng để thống kê món ăn/đồ uống yêu thích
        {
            try
            {
                string sql_favorite_food = "select MENU_FOOD.Name_Food as N'Tên món ăn', sum(NOF) as N'Số lần được gọi' from ORDER_FOOD,MENU_FOOD,BILL where ORDER_FOOD.ID_Food = MENU_FOOD.ID_Food and ORDER_FOOD.ID_Bill = BILL.ID_Bill and month(BILL.Times) = " + Convert.ToString(cbbChonThang.SelectedIndex+1)+ " group by MENU_FOOD.Name_Food ";
                SqlDataAdapter daFavoriteFood = new SqlDataAdapter(sql_favorite_food, conn);
                DataTable dtFavoriteFood = new DataTable();
                daFavoriteFood.Fill(dtFavoriteFood);
                dgvFavoriteFood.DataSource = dtFavoriteFood;
                groupBox5.Text = "Danh sách món ăn được yêu thích nhất trong " + cbbChonThang.Text;
            }
            catch
            {
                MessageBox.Show("Lỗi gì đó");
            }

            try
            {
                string sql_favorite_drink = "select MENU_DRINK.Name_Drink  as N'Tên món ăn', sum(NOD) as N'Số lần được gọi' from ORDER_DRINK,MENU_DRINK,BILL where ORDER_DRINK.ID_Drink = MENU_DRINK.ID_Drink and ORDER_DRINK.ID_Bill = BILL.ID_Bill and month(BILL.Times) = " + Convert.ToString(cbbChonThang.SelectedIndex + 1) + " group by MENU_DRINK.Name_Drink ";
                SqlDataAdapter daFavoriteDrink = new SqlDataAdapter(sql_favorite_drink, conn);
                DataTable dtFavoriteDrink = new DataTable();
                daFavoriteDrink.Fill(dtFavoriteDrink);
                dgvFavoriteDrink.DataSource = dtFavoriteDrink;
                groupBox6.Text = "Các món ăn được yêu thích nhất trong " + cbbChonThang.Text;
            }
            catch
            {
                MessageBox.Show("Lỗi gì đó");
            }
        }

    }

}
