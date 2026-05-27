using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EventVenueApp
{
    // ============================================================
    //  Main Form  -  Event & Venue Management System
    //  Covers all project requirements:
    //    2 INSERT  (Events + Patrons)
    //    2 DELETE  (Events + Staff)
    //    2 UPDATE  (Events + Patrons)
    //    SELECT from single tables  (all tabs)
    //    SELECT with JOINs          (Reports tab)
    //    Full GUI                   (bonus)
    // ============================================================
    public class MainForm : Form
    {
        private TabControl tabMain;

        public MainForm()
        {
            this.Text = "Event & Venue Management System";
            this.Size = new Size(970, 680);
            this.MinimumSize = new Size(860, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 9F);

            BuildUI();
        }

        // ─────────────────────────────────────────────────────
        //  UI BUILDER
        // ─────────────────────────────────────────────────────
        private void BuildUI()
        {
            var header = new Label
            {
                Text = "  Event & Venue Management System",
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(30, 60, 120),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            tabMain = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Padding = new Point(14, 4)
            };

            tabMain.TabPages.Add(BuildEventsTab());
            tabMain.TabPages.Add(BuildPatronsTab());
            tabMain.TabPages.Add(BuildVenuesTab());
            tabMain.TabPages.Add(BuildStaffTab());
            tabMain.TabPages.Add(BuildTicketsTab());
            tabMain.TabPages.Add(BuildTicketTypesTab());
            tabMain.TabPages.Add(BuildReportsTab());

            // ORDER MATTERS: TabControl first, then header
            this.Controls.Add(tabMain);
            this.Controls.Add(header);
        }

        // ─────────────────────────────────────────────────────
        //  SHARED HELPERS
        // ─────────────────────────────────────────────────────

        /// <summary>Creates a styled DataGridView.</summary>
        private DataGridView MakeGrid(Panel parent, int top, int height)
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,   // ← changed from fixed location
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9F),
                GridColor = Color.FromArgb(220, 225, 235),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(40, 40, 60),
                    Padding = new Padding(3)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 249, 255)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(30, 60, 120),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Padding = new Padding(3)
                }
            };
            dgv.EnableHeadersVisualStyles = false;
            parent.Controls.Add(dgv);
            return dgv;
        }

        /// <summary>Creates a label + textbox pair and returns the TextBox.</summary>
        private TextBox MakeField(string label, int x, int y, Panel parent, int width = 130)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(80, 90, 110)
            });
            var txt = new TextBox
            {
                Location = new Point(x, y + 19),
                Width = width,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle
            };
            parent.Controls.Add(txt);
            return txt;
        }

        /// <summary>Creates a styled action button.</summary>
        private Button MakeBtn(string text, Color back, int x, int y, Panel parent)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(125, 32),
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            parent.Controls.Add(btn);
            return btn;
        }

        /// <summary>Creates the bottom input panel with a title.</summary>
        private Panel MakeInputPanel(TabPage page, int top, string title)
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Bottom,   // ← changed from fixed location
                Height = 160,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnl.Controls.Add(new Label
            {
                Text = title,
                Location = new Point(10, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 120)
            });
            page.Controls.Add(pnl);
            return pnl;
        }

        private void OK(string msg) =>
            MessageBox.Show(msg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void Err(string msg) =>
            MessageBox.Show(msg, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private bool Empty(params TextBox[] fields)
        {
            foreach (var f in fields)
                if (string.IsNullOrWhiteSpace(f.Text)) return true;
            return false;
        }

        // ─────────────────────────────────────────────────────
        //  TAB 1 : EVENTS
        // ─────────────────────────────────────────────────────
        private TabPage BuildEventsTab()
        {
            var page = new TabPage("  Events  ");
            var scroll = new Panel { Dock = DockStyle.Fill };
            page.Controls.Add(scroll);

            var dgv = MakeGrid(scroll, 10, 310);
            var pnl = MakeInputPanel(page, 330, "Event Operations");

            // Fields
            var txtID    = MakeField("Event ID",         10,  30, pnl);
            var txtName  = MakeField("Event Name",      155,  30, pnl, 160);
            var txtDate  = MakeField("Date (YYYY-MM-DD)",330, 30, pnl, 150);
            var txtVenue = MakeField("Venue ID",         495, 30, pnl);

            // Buttons
            var btnLoad   = MakeBtn("Load All",    Color.FromArgb(30, 60, 120),   10, 95, pnl);
            var btnInsert = MakeBtn("Insert",       Color.FromArgb(39, 174, 96),  145, 95, pnl);
            var btnDelete = MakeBtn("Delete by ID", Color.FromArgb(192, 57, 43), 280, 95, pnl);
            var btnUpdate = MakeBtn("Update Name",  Color.FromArgb(211, 84, 0),   415, 95, pnl);

            // ── Load ──
            btnLoad.Click += (s, e) =>
                dgv.DataSource = DBHelper.GetData("SELECT * FROM EVENT ORDER BY Date");

            // ── INSERT (Requirement 1) ──
            btnInsert.Click += (s, e) =>
            {
                if (Empty(txtID, txtName, txtDate, txtVenue)) { Err("Fill all fields."); return; }
                if (!int.TryParse(txtID.Text, out int eid))   { Err("Event ID must be a number."); return; }
                if (!int.TryParse(txtVenue.Text, out int vid)){ Err("Venue ID must be a number."); return; }
                if (!DateTime.TryParse(txtDate.Text, out DateTime dt)) { Err("Invalid date format."); return; }

                string sql = "INSERT INTO EVENT (EventID, Name, Date, VenueID) VALUES (@id,@name,@date,@venue)";
                var p = new[]
                {
                    new SqlParameter("@id",    eid),
                    new SqlParameter("@name",  txtName.Text.Trim()),
                    new SqlParameter("@date",  dt),
                    new SqlParameter("@venue", vid)
                };
                if (DBHelper.Execute(sql, p)) { OK("Event inserted!"); btnLoad.PerformClick(); }
            };

            // ── DELETE (Requirement 1) ──
            btnDelete.Click += (s, e) =>
            {
                if (Empty(txtID)) { Err("Enter Event ID to delete."); return; }
                if (!int.TryParse(txtID.Text, out int eid)) { Err("Event ID must be a number."); return; }
                if (MessageBox.Show($"Delete Event #{eid}?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DBHelper.Execute("DELETE FROM EVENT WHERE EventID=@id",
                        new[] { new SqlParameter("@id", eid) }))
                    { OK("Event deleted."); btnLoad.PerformClick(); }
                }
            };

            // ── UPDATE (Requirement 1) ──
            btnUpdate.Click += (s, e) =>
            {
                if (Empty(txtID, txtName)) { Err("Enter Event ID and new Name."); return; }
                if (!int.TryParse(txtID.Text, out int eid)) { Err("Event ID must be a number."); return; }
                string sql = "UPDATE EVENT SET Name=@name WHERE EventID=@id";
                var p = new[] { new SqlParameter("@name", txtName.Text.Trim()), new SqlParameter("@id", eid) };
                if (DBHelper.Execute(sql, p)) { OK("Event name updated."); btnLoad.PerformClick(); }
            };

            return page;
        }

        // ─────────────────────────────────────────────────────
        //  TAB 2 : PATRONS
        // ─────────────────────────────────────────────────────
        private TabPage BuildPatronsTab()
        {
            var page = new TabPage("  Patrons  ");
            var scroll = new Panel { Dock = DockStyle.Fill };
            page.Controls.Add(scroll);

            var dgv = MakeGrid(scroll, 10, 310);
            var pnl = MakeInputPanel(page, 330, "Patron Operations");

            var txtID    = MakeField("Patron ID",  10, 30, pnl);
            var txtName  = MakeField("Name",      155, 30, pnl, 160);
            var txtEmail = MakeField("Email",      330, 30, pnl, 200);

            var btnLoad   = MakeBtn("Load All",      Color.FromArgb(30, 60, 120),   10, 95, pnl);
            var btnInsert = MakeBtn("Insert",         Color.FromArgb(39, 174, 96),  145, 95, pnl);
            var btnDelete = MakeBtn("Delete by ID",   Color.FromArgb(192, 57, 43), 280, 95, pnl);
            var btnUpdate = MakeBtn("Update Email",   Color.FromArgb(211, 84, 0),   415, 95, pnl);

            btnLoad.Click += (s, e) =>
                dgv.DataSource = DBHelper.GetData("SELECT * FROM PATRON ORDER BY Name");

            // ── INSERT (Requirement 2) ──
            btnInsert.Click += (s, e) =>
            {
                if (Empty(txtID, txtName, txtEmail)) { Err("Fill all fields."); return; }
                if (!int.TryParse(txtID.Text, out int pid)) { Err("Patron ID must be a number."); return; }
                string sql = "INSERT INTO PATRON (PatronID, Name, Email) VALUES (@id,@name,@email)";
                var p = new[]
                {
                    new SqlParameter("@id",    pid),
                    new SqlParameter("@name",  txtName.Text.Trim()),
                    new SqlParameter("@email", txtEmail.Text.Trim())
                };
                if (DBHelper.Execute(sql, p)) { OK("Patron added!"); btnLoad.PerformClick(); }
            };

            // ── DELETE (Requirement 2) ──
            btnDelete.Click += (s, e) =>
            {
                if (Empty(txtID)) { Err("Enter Patron ID."); return; }
                if (!int.TryParse(txtID.Text, out int pid)) { Err("Patron ID must be a number."); return; }
                if (MessageBox.Show($"Delete Patron #{pid}?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DBHelper.Execute("DELETE FROM PATRON WHERE PatronID=@id",
                        new[] { new SqlParameter("@id", pid) }))
                    { OK("Patron deleted."); btnLoad.PerformClick(); }
                }
            };

            // ── UPDATE (Requirement 2) ──
            btnUpdate.Click += (s, e) =>
            {
                if (Empty(txtID, txtEmail)) { Err("Enter Patron ID and new Email."); return; }
                if (!int.TryParse(txtID.Text, out int pid)) { Err("Patron ID must be a number."); return; }
                string sql = "UPDATE PATRON SET Email=@email WHERE PatronID=@id";
                var p = new[] { new SqlParameter("@email", txtEmail.Text.Trim()), new SqlParameter("@id", pid) };
                if (DBHelper.Execute(sql, p)) { OK("Email updated."); btnLoad.PerformClick(); }
            };

            return page;
        }

        // ─────────────────────────────────────────────────────
        //  TAB 3 : VENUES
        // ─────────────────────────────────────────────────────
        private TabPage BuildVenuesTab()
        {
            var page = new TabPage("  Venues  ");
            var scroll = new Panel { Dock = DockStyle.Fill };
            page.Controls.Add(scroll);

            var dgv = MakeGrid(scroll, 10, 310);
            var pnl = MakeInputPanel(page, 330, "Venue Operations");

            var txtID  = MakeField("Venue ID",   10, 30, pnl);
            var txtLoc = MakeField("Location",  155, 30, pnl, 200);
            var txtCap = MakeField("Capacity",  370, 30, pnl);

            var btnLoad   = MakeBtn("Load All",      Color.FromArgb(30, 60, 120),   10, 95, pnl);
            var btnInsert = MakeBtn("Insert",         Color.FromArgb(39, 174, 96),  145, 95, pnl);
            var btnDelete = MakeBtn("Delete by ID",   Color.FromArgb(192, 57, 43), 280, 95, pnl);
            var btnUpdate = MakeBtn("Update Capacity",Color.FromArgb(211, 84, 0),   415, 95, pnl);

            btnLoad.Click += (s, e) =>
                dgv.DataSource = DBHelper.GetData("SELECT * FROM VENUE ORDER BY Loacation");

            btnInsert.Click += (s, e) =>
            {
                if (Empty(txtID, txtLoc, txtCap)) { Err("Fill all fields."); return; }
                if (!int.TryParse(txtID.Text, out int vid))  { Err("Venue ID must be a number."); return; }
                if (!int.TryParse(txtCap.Text, out int cap)) { Err("Capacity must be a number."); return; }
                // Note: column is misspelled "Loacation" in the original DDL
                string sql = "INSERT INTO VENUE (VenueID, Loacation, Capacity) VALUES (@id,@loc,@cap)";
                var p = new[]
                {
                    new SqlParameter("@id",  vid),
                    new SqlParameter("@loc", txtLoc.Text.Trim()),
                    new SqlParameter("@cap", cap)
                };
                if (DBHelper.Execute(sql, p)) { OK("Venue added!"); btnLoad.PerformClick(); }
            };

            btnDelete.Click += (s, e) =>
            {
                if (Empty(txtID)) { Err("Enter Venue ID."); return; }
                if (!int.TryParse(txtID.Text, out int vid)) { Err("Venue ID must be a number."); return; }
                if (MessageBox.Show($"Delete Venue #{vid}?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DBHelper.Execute("DELETE FROM VENUE WHERE VenueID=@id",
                        new[] { new SqlParameter("@id", vid) }))
                    { OK("Venue deleted."); btnLoad.PerformClick(); }
                }
            };

            btnUpdate.Click += (s, e) =>
            {
                if (Empty(txtID, txtCap)) { Err("Enter Venue ID and new Capacity."); return; }
                if (!int.TryParse(txtID.Text, out int vid))  { Err("Venue ID must be a number."); return; }
                if (!int.TryParse(txtCap.Text, out int cap)) { Err("Capacity must be a number."); return; }
                if (DBHelper.Execute("UPDATE VENUE SET Capacity=@cap WHERE VenueID=@id",
                    new[] { new SqlParameter("@cap", cap), new SqlParameter("@id", vid) }))
                { OK("Capacity updated."); btnLoad.PerformClick(); }
            };

            return page;
        }

        // ─────────────────────────────────────────────────────
        //  TAB 4 : STAFF
        // ─────────────────────────────────────────────────────
        private TabPage BuildStaffTab()
        {
            var page = new TabPage("  Staff  ");
            var scroll = new Panel { Dock = DockStyle.Fill };
            page.Controls.Add(scroll);

            var dgv = MakeGrid(scroll, 10, 310);
            var pnl = MakeInputPanel(page, 330, "Staff Operations");

            var txtID    = MakeField("Staff ID",                    10, 30, pnl);
            var txtName  = MakeField("Name",                       155, 30, pnl, 150);
            var txtPhone = MakeField("Phone",                       320, 30, pnl);
            var txtRole  = MakeField("Role (Coordinator/Technician)", 465, 30, pnl, 180);

            var btnLoad   = MakeBtn("Load All",    Color.FromArgb(30, 60, 120),    10, 95, pnl);
            var btnInsert = MakeBtn("Insert",       Color.FromArgb(39, 174, 96),   145, 95, pnl);
            var btnDelete = MakeBtn("Delete by ID", Color.FromArgb(192, 57, 43),  280, 95, pnl);
            var btnUpdate = MakeBtn("Update Role",  Color.FromArgb(211, 84, 0),    415, 95, pnl);

            btnLoad.Click += (s, e) =>
                dgv.DataSource = DBHelper.GetData("SELECT * FROM STAFF ORDER BY Staff_Name");

            btnInsert.Click += (s, e) =>
            {
                if (Empty(txtID, txtName, txtRole)) { Err("Fill ID, Name and Role."); return; }
                if (!int.TryParse(txtID.Text, out int sid)) { Err("Staff ID must be a number."); return; }
                string sql = "INSERT INTO STAFF (StaffID, Phone, Role, Staff_Name) VALUES (@id,@phone,@role,@name)";
                var p = new[]
                {
                    new SqlParameter("@id",    sid),
                    new SqlParameter("@phone", string.IsNullOrWhiteSpace(txtPhone.Text)
                                               ? (object)DBNull.Value : txtPhone.Text.Trim()),
                    new SqlParameter("@role",  txtRole.Text.Trim()),
                    new SqlParameter("@name",  txtName.Text.Trim())
                };
                if (DBHelper.Execute(sql, p)) { OK("Staff member added!"); btnLoad.PerformClick(); }
            };

            btnDelete.Click += (s, e) =>
            {
                if (Empty(txtID)) { Err("Enter Staff ID."); return; }
                if (!int.TryParse(txtID.Text, out int sid)) { Err("Staff ID must be a number."); return; }
                if (MessageBox.Show($"Delete Staff #{sid}?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DBHelper.Execute("DELETE FROM STAFF WHERE StaffID=@id",
                        new[] { new SqlParameter("@id", sid) }))
                    { OK("Staff member deleted."); btnLoad.PerformClick(); }
                }
            };

            btnUpdate.Click += (s, e) =>
            {
                if (Empty(txtID, txtRole)) { Err("Enter Staff ID and new Role."); return; }
                if (!int.TryParse(txtID.Text, out int sid)) { Err("Staff ID must be a number."); return; }
                if (DBHelper.Execute("UPDATE STAFF SET Role=@role WHERE StaffID=@id",
                    new[] { new SqlParameter("@role", txtRole.Text.Trim()), new SqlParameter("@id", sid) }))
                { OK("Staff role updated."); btnLoad.PerformClick(); }
            };

            return page;
        }

        // ─────────────────────────────────────────────────────
        //  TAB 5 : TICKETS
        // ─────────────────────────────────────────────────────
        private TabPage BuildTicketsTab()
        {
            var page = new TabPage("  Tickets  ");
            var scroll = new Panel { Dock = DockStyle.Fill };
            page.Controls.Add(scroll);

            var dgv = MakeGrid(scroll, 10, 310);
            var pnl = MakeInputPanel(page, 330, "Ticket Operations");

            var txtTID  = MakeField("Ticket ID",      10, 30, pnl);
            var txtPID  = MakeField("Patron ID",      155, 30, pnl);
            var txtEID  = MakeField("Event ID",       300, 30, pnl);
            var txtTTID = MakeField("Ticket Type ID", 445, 30, pnl);

            var btnLoad   = MakeBtn("Load All",    Color.FromArgb(30, 60, 120),   10, 95, pnl);
            var btnInsert = MakeBtn("Insert",       Color.FromArgb(39, 174, 96),  145, 95, pnl);
            var btnDelete = MakeBtn("Delete by ID", Color.FromArgb(192, 57, 43), 280, 95, pnl);

            btnLoad.Click += (s, e) =>
                dgv.DataSource = DBHelper.GetData("SELECT * FROM TICKET ORDER BY TicketID");

            btnInsert.Click += (s, e) =>
            {
                if (Empty(txtTID, txtPID, txtEID, txtTTID)) { Err("Fill all fields."); return; }
                if (!int.TryParse(txtTID.Text, out int tid) ||
                    !int.TryParse(txtPID.Text, out int pid) ||
                    !int.TryParse(txtEID.Text, out int eid) ||
                    !int.TryParse(txtTTID.Text, out int ttid))
                { Err("All IDs must be numbers."); return; }

                string sql = "INSERT INTO TICKET (TicketID, PatronID, EventID, TicketTypeID) VALUES (@tid,@pid,@eid,@ttid)";
                var p = new[]
                {
                    new SqlParameter("@tid",  tid),
                    new SqlParameter("@pid",  pid),
                    new SqlParameter("@eid",  eid),
                    new SqlParameter("@ttid", ttid)
                };
                if (DBHelper.Execute(sql, p)) { OK("Ticket issued!"); btnLoad.PerformClick(); }
            };

            btnDelete.Click += (s, e) =>
            {
                if (Empty(txtTID)) { Err("Enter Ticket ID."); return; }
                if (!int.TryParse(txtTID.Text, out int tid)) { Err("Ticket ID must be a number."); return; }
                if (MessageBox.Show($"Delete Ticket #{tid}?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DBHelper.Execute("DELETE FROM TICKET WHERE TicketID=@id",
                        new[] { new SqlParameter("@id", tid) }))
                    { OK("Ticket deleted."); btnLoad.PerformClick(); }
                }
            };

            return page;
        }

        // ─────────────────────────────────────────────────────
        //  TAB 6 : TICKET TYPES
        // ─────────────────────────────────────────────────────
        private TabPage BuildTicketTypesTab()
        {
            var page = new TabPage("  Ticket Types  ");
            var scroll = new Panel { Dock = DockStyle.Fill };
            page.Controls.Add(scroll);

            var dgv = MakeGrid(scroll, 10, 310);
            var pnl = MakeInputPanel(page, 330, "Ticket Type Operations");

            var txtID    = MakeField("Type ID",   10, 30, pnl);
            var txtType  = MakeField("Type Name", 155, 30, pnl, 130);
            var txtPrice = MakeField("Price",     300, 30, pnl, 100);
            var txtSeats = MakeField("Seats",     415, 30, pnl, 100);
            var txtEID   = MakeField("Event ID",  530, 30, pnl, 100);

            var btnLoad   = MakeBtn("Load All",    Color.FromArgb(30, 60, 120),   10, 95, pnl);
            var btnInsert = MakeBtn("Insert",       Color.FromArgb(39, 174, 96),  145, 95, pnl);
            var btnDelete = MakeBtn("Delete by ID", Color.FromArgb(192, 57, 43), 280, 95, pnl);

            btnLoad.Click += (s, e) =>
                dgv.DataSource = DBHelper.GetData("SELECT * FROM TICKET_TYPE ORDER BY EventID");

            btnInsert.Click += (s, e) =>
            {
                if (Empty(txtID, txtType, txtPrice, txtSeats, txtEID)) { Err("Fill all fields."); return; }
                if (!int.TryParse(txtID.Text, out int id) ||
                    !int.TryParse(txtSeats.Text, out int seats) ||
                    !int.TryParse(txtEID.Text, out int eid) ||
                    !float.TryParse(txtPrice.Text, out float price))
                { Err("Numeric fields must be numbers."); return; }

                string sql = "INSERT INTO TICKET_TYPE (TicketTypeID, TypeName, Price, Seats, EventID) VALUES (@id,@type,@price,@seats,@eid)";
                var p = new[]
                {
                    new SqlParameter("@id",    id),
                    new SqlParameter("@type",  txtType.Text.Trim()),
                    new SqlParameter("@price", price),
                    new SqlParameter("@seats", seats),
                    new SqlParameter("@eid",   eid)
                };
                if (DBHelper.Execute(sql, p)) { OK("Ticket type added!"); btnLoad.PerformClick(); }
            };

            btnDelete.Click += (s, e) =>
            {
                if (Empty(txtID)) { Err("Enter Type ID."); return; }
                if (!int.TryParse(txtID.Text, out int id)) { Err("ID must be a number."); return; }
                if (MessageBox.Show($"Delete Ticket Type #{id}?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DBHelper.Execute("DELETE FROM TICKET_TYPE WHERE TicketTypeID=@id",
                        new[] { new SqlParameter("@id", id) }))
                    { OK("Ticket type deleted."); btnLoad.PerformClick(); }
                }
            };

            return page;
        }

        // ─────────────────────────────────────────────────────
        //  TAB 7 : REPORTS  (JOIN Queries)
        // ─────────────────────────────────────────────────────
        private TabPage BuildReportsTab()
        {
            var page = new TabPage("  Reports  ");

            // Top toolbar
            var toolbar = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(930, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            toolbar.Controls.Add(new Label
            {
                Text = "Select Report:",
                Location = new Point(10, 15),
                AutoSize = true,
                ForeColor = Color.FromArgb(80, 90, 110)
            });

            var cmb = new ComboBox
            {
                Location = new Point(100, 12),
                Width = 550,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmb.Items.AddRange(new string[]
            {
                "1. Tickets  -  Patron Name + Event Name + Ticket Type (JOIN x3)",
                "2. Events   -  With Venue Location & Capacity (JOIN)",
                "3. Staff    -  Staff Assigned to Each Event (JOIN)",
                "4. Patrons  -  Total Tickets per Patron (LEFT JOIN + GROUP BY)",
                "5. Events   -  All Ticket Types & Prices per Event (JOIN)"
            });
            cmb.SelectedIndex = 0;
            toolbar.Controls.Add(cmb);

            var btnRun = new Button
            {
                Text = "Run Report",
                Location = new Point(665, 9),
                Size = new Size(120, 32),
                BackColor = Color.FromArgb(30, 60, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRun.FlatAppearance.BorderSize = 0;
            toolbar.Controls.Add(btnRun);

            page.Controls.Add(toolbar);

            // Grid
            var dgv = MakeGrid(page, 70, 520);
            dgv.Location = new Point(10, 70);
            dgv.Size = new Size(930, 520);
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // ── Run Report ──
            btnRun.Click += (s, e) =>
            {
                string sql = "";
                switch (cmb.SelectedIndex)
                {
                    case 0: // Tickets with patron, event, ticket type
                        sql = @"
                            SELECT
                                T.TicketID,
                                P.Name        AS PatronName,
                                P.Email       AS PatronEmail,
                                E.Name        AS EventName,
                                E.Date        AS EventDate,
                                TT.TypeName   AS TicketType,
                                TT.Price
                            FROM TICKET T
                            JOIN PATRON      P  ON T.PatronID     = P.PatronID
                            JOIN EVENT       E  ON T.EventID      = E.EventID
                            JOIN TICKET_TYPE TT ON T.TicketTypeID = TT.TicketTypeID
                            ORDER BY E.Date, P.Name";
                        break;

                    case 1: // Events with venue
                        sql = @"
                            SELECT
                                E.EventID,
                                E.Name        AS EventName,
                                E.Date,
                                V.Loacation   AS VenueLocation,
                                V.Capacity    AS VenueCapacity
                            FROM EVENT E
                            JOIN VENUE V ON E.VenueID = V.VenueID
                            ORDER BY E.Date";
                        break;

                    case 2: // Staff per event
                        sql = @"
                            SELECT
                                E.Name        AS EventName,
                                E.Date,
                                S.Staff_Name  AS StaffName,
                                S.Role,
                                S.Phone
                            FROM Event_Staff ES
                            JOIN EVENT E ON ES.EventID = E.EventID
                            JOIN STAFF S  ON ES.StaffID = S.StaffID
                            ORDER BY E.Name, S.Role";
                        break;

                    case 3: // Patron ticket count
                        sql = @"
                            SELECT
                                P.PatronID,
                                P.Name,
                                P.Email,
                                COUNT(T.TicketID) AS TotalTickets
                            FROM PATRON P
                            LEFT JOIN TICKET T ON P.PatronID = T.PatronID
                            GROUP BY P.PatronID, P.Name, P.Email
                            ORDER BY TotalTickets DESC";
                        break;

                    case 4: // Ticket types per event
                        sql = @"
                            SELECT
                                E.Name        AS EventName,
                                E.Date,
                                TT.TypeName   AS TicketType,
                                TT.Price,
                                TT.Seats      AS AvailableSeats
                            FROM TICKET_TYPE TT
                            JOIN EVENT E ON TT.EventID = E.EventID
                            ORDER BY E.Name, TT.Price DESC";
                        break;
                }

                if (!string.IsNullOrEmpty(sql))
                    dgv.DataSource = DBHelper.GetData(sql);
            };

            return page;
        }
    }
}
