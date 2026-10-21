using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace StudentManager
{
    public class Form1 : Form
    {
        private readonly BindingList<Student> students = new();
        private readonly BindingSource source = new();

        private readonly TextBox txtId = new();
        private readonly TextBox txtName = new();
        private readonly TextBox txtAge = new();
        private readonly ComboBox cboGender = new();
        private readonly PictureBox picAvatar = new();
        private readonly DataGridView grid = new();
        private readonly Label lblAvatar = new();

        private string selectedAvatarPath = "";

        public Form1()
        {
            Text = "BÀI 4.6 - QUẢN LÝ SINH VIÊN";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 600);
            Size = new Size(1100, 700);
            Font = new Font("Segoe UI", 10F);
            BuildInterface();
            LoadSampleData();
            SetupBinding();
        }

        private void BuildInterface()
        {
            var main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(12)
            };
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 270));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(main);

            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            main.Controls.Add(top, 0, 0);

            var inputGroup = new GroupBox
            {
                Text = "Thông tin sinh viên",
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };
            top.Controls.Add(inputGroup, 0, 0);

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 5; i++)
                fields.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            inputGroup.Controls.Add(fields);

            AddField(fields, "Mã sinh viên:", txtId, 0);
            AddField(fields, "Họ và tên:", txtName, 1);
            AddField(fields, "Tuổi:", txtAge, 2);

            cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
            cboGender.Items.AddRange(new object[] { "Nam", "Nữ" });
            cboGender.SelectedIndex = 0;
            AddField(fields, "Giới tính:", cboGender, 3);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };
            var btnAdd = MakeButton("Thêm", (_, _) => AddStudent());
            var btnUpdate = MakeButton("Cập nhật", (_, _) => UpdateStudent());
            var btnDelete = MakeButton("Xóa", (_, _) => DeleteStudent());
            var btnReset = MakeButton("Làm mới", (_, _) => ClearInputs());
            buttons.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnReset });
            fields.Controls.Add(buttons, 1, 4);

            var avatarGroup = new GroupBox
            {
                Text = "Ảnh đại diện",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            top.Controls.Add(avatarGroup, 1, 0);

            picAvatar.Dock = DockStyle.Fill;
            picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            picAvatar.BorderStyle = BorderStyle.FixedSingle;
            picAvatar.BackColor = Color.WhiteSmoke;

            var avatarLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            avatarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            avatarLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            avatarLayout.Controls.Add(picAvatar, 0, 0);

            var btnChooseAvatar = MakeButton("Chọn ảnh...", (_, _) => ChooseAvatar());
            btnChooseAvatar.Dock = DockStyle.Fill;
            avatarLayout.Controls.Add(btnChooseAvatar, 0, 1);
            avatarGroup.Controls.Add(avatarLayout);

            var gridGroup = new GroupBox
            {
                Text = "Danh sách sinh viên",
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };
            main.Controls.Add(gridGroup, 0, 1);

            grid.Dock = DockStyle.Fill;
            grid.AutoGenerateColumns = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Student.StudentId), HeaderText = "Mã sinh viên", FillWeight = 18 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Student.FullName), HeaderText = "Họ và tên", FillWeight = 34 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Student.Age), HeaderText = "Tuổi", FillWeight = 10 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Student.Gender), HeaderText = "Giới tính", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Student.AvatarPath), HeaderText = "Đường dẫn ảnh", FillWeight = 35 });
            grid.SelectionChanged += (_, _) => DisplaySelectedStudent();
            gridGroup.Controls.Add(grid);

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 48,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(4)
            };
            var btnExport = MakeButton("Xuất CSV", (_, _) => ExportCsv());
            bottom.Controls.Add(btnExport);
            gridGroup.Controls.Add(bottom);
            bottom.BringToFront();

            FormClosed += (_, _) =>
            {
                var old = picAvatar.Image;
                picAvatar.Image = null;
                old?.Dispose();
            };
        }

        private static void AddField(TableLayoutPanel panel, string label, Control control, int row)
        {
            var lbl = new Label
            {
                Text = label,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            control.Dock = DockStyle.Fill;
            if (control is TextBox textBox)
                textBox.Margin = new Padding(3, 5, 8, 5);
            panel.Controls.Add(lbl, 0, row);
            panel.Controls.Add(control, 1, row);
        }

        private static Button MakeButton(string text, EventHandler click)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                MinimumSize = new Size(90, 32),
                Margin = new Padding(4)
            };
            button.Click += click;
            return button;
        }

        private void LoadSampleData()
        {
            students.Add(new Student { StudentId = "SV001", FullName = "Nguyễn Minh Anh", Age = 20, Gender = "Nam" });
            students.Add(new Student { StudentId = "SV002", FullName = "Trần Thu Hà", Age = 19, Gender = "Nữ" });
        }

        private void SetupBinding()
        {
            source.DataSource = students;
            grid.DataSource = source;
            source.CurrentChanged += (_, _) => DisplaySelectedStudent();
            if (grid.Rows.Count > 0)
                grid.Rows[0].Selected = true;
            DisplaySelectedStudent();
        }

        private void AddStudent()
        {
            if (!TryReadInput(out Student? student))
                return;

            if (students.Any(s => s.StudentId.Equals(student.StudentId, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sinh viên đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            students.Add(student);
            source.Position = students.Count - 1;
            MessageBox.Show("Đã thêm sinh viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateStudent()
        {
            if (source.Current is not Student current)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần cập nhật.");
                return;
            }

            if (!TryReadInput(out Student? edited))
                return;

            if (students.Any(s => !ReferenceEquals(s, current) &&
                s.StudentId.Equals(edited.StudentId, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mã sinh viên đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            current.StudentId = edited.StudentId;
            current.FullName = edited.FullName;
            current.Age = edited.Age;
            current.Gender = edited.Gender;
            current.AvatarPath = edited.AvatarPath;
            source.ResetCurrentItem();
            DisplaySelectedStudent();
            MessageBox.Show("Đã cập nhật sinh viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteStudent()
        {
            if (source.Current is not Student current)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa.");
                return;
            }

            var answer = MessageBox.Show($"Bạn có chắc muốn xóa {current.FullName}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer == DialogResult.Yes)
            {
                int index = source.Position;
                students.Remove(current);
                if (students.Count > 0)
                    source.Position = Math.Min(index, students.Count - 1);
                else
                    ClearInputs();
            }
        }

        private bool TryReadInput(out Student student)
        {
            student = new Student();
            string id = txtId.Text.Trim();
            string name = txtName.Text.Trim();

            if (id.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên.");
                txtId.Focus();
                return false;
            }
            if (name.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập họ và tên.");
                txtName.Focus();
                return false;
            }
            if (!int.TryParse(txtAge.Text.Trim(), out int age) || age < 16 || age > 100)
            {
                MessageBox.Show("Tuổi phải là số nguyên từ 16 đến 100.");
                txtAge.Focus();
                return false;
            }

            student = new Student
            {
                StudentId = id,
                FullName = name,
                Age = age,
                Gender = cboGender.SelectedItem?.ToString() ?? "Nam",
                AvatarPath = selectedAvatarPath
            };
            return true;
        }

        private void ChooseAvatar()
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Chọn ảnh đại diện",
                Filter = "Tệp ảnh|*.jpg;*.jpeg;*.png;*.bmp|JPEG|*.jpg;*.jpeg|PNG|*.png|Bitmap|*.bmp",
                Multiselect = false
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                selectedAvatarPath = dialog.FileName;
                LoadAvatar(selectedAvatarPath);
            }
        }

        private void LoadAvatar(string path)
        {
            Image? newImage = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var temp = Image.FromStream(stream);
                    newImage = new Bitmap(temp);
                }
            }
            catch
            {
                newImage = null;
            }

            Image? oldImage = picAvatar.Image;
            picAvatar.Image = newImage;
            oldImage?.Dispose();
        }

        private void DisplaySelectedStudent()
        {
            if (source.Current is not Student student)
            {
                txtId.Text = "";
                txtName.Text = "";
                txtAge.Text = "";
                selectedAvatarPath = "";
                LoadAvatar("");
                return;
            }

            txtId.Text = student.StudentId;
            txtName.Text = student.FullName;
            txtAge.Text = student.Age.ToString();
            cboGender.SelectedItem = student.Gender;
            selectedAvatarPath = student.AvatarPath;
            LoadAvatar(student.AvatarPath);
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtName.Clear();
            txtAge.Clear();
            cboGender.SelectedIndex = 0;
            selectedAvatarPath = "";
            LoadAvatar("");
            txtId.Focus();
        }

        private void ExportCsv()
        {
            using var dialog = new SaveFileDialog
            {
                Title = "Lưu danh sách sinh viên",
                Filter = "CSV UTF-8 (*.csv)|*.csv",
                FileName = "DanhSachSinhVien.csv",
                DefaultExt = "csv",
                AddExtension = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                var lines = new List<string>
                {
                    "MaSinhVien,HoVaTen,Tuoi,GioiTinh,DuongDanAnh"
                };

                foreach (Student s in students)
                {
                    lines.Add(string.Join(",", new[]
                    {
                        EscapeCsv(s.StudentId),
                        EscapeCsv(s.FullName),
                        s.Age.ToString(),
                        EscapeCsv(s.Gender),
                        EscapeCsv(s.AvatarPath)
                    }));
                }

                File.WriteAllLines(dialog.FileName, lines, new UTF8Encoding(true));
                MessageBox.Show("Xuất CSV thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xuất CSV: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string EscapeCsv(string value)
        {
            value ??= "";
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
