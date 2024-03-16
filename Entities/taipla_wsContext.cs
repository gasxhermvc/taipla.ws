using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Taipla.Webservice.Entities
{
    public partial class xxxContext : DbContext
    {
        public xxxContext()
        {
        }

        public xxxContext(DbContextOptions<xxxContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<FoodCenter> FoodCenter { get; set; }
        public virtual DbSet<FoodCountry> FoodCountry { get; set; }
        public virtual DbSet<FoodCulture> FoodCulture { get; set; }
        public virtual DbSet<FoodIngredient> FoodIngredient { get; set; }
        public virtual DbSet<HistorySearch> HistorySearch { get; set; }
        public virtual DbSet<Legend> Legend { get; set; }
        public virtual DbSet<Media> Media { get; set; }
        public virtual DbSet<Promotion> Promotion { get; set; }
        public virtual DbSet<Restaurant> Restaurant { get; set; }
        public virtual DbSet<RestaurantIngredient> RestaurantIngredient { get; set; }
        public virtual DbSet<RestaurantMenu> RestaurantMenu { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserDevice> UserDevice { get; set; }
        public virtual DbSet<Vote> Vote { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.CmtId)
                    .HasName("PRIMARY");

                entity.ToTable("comment");

                entity.HasComment("ตารางเก็บข้อมูลคอมเม้นต์");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("cmt_created_date_idx");

                entity.HasIndex(e => e.ImageStatus)
                    .HasName("cmt_image_status_idx");

                entity.HasIndex(e => e.RefId)
                    .HasName("cmt_ref_id_idx");

                entity.HasIndex(e => e.SystemName)
                    .HasName("cmt_system_name_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("cmt_user_id_idx");

                entity.Property(e => e.CmtId)
                    .HasColumnName("cmt_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสคอมเม้นต์");

                entity.Property(e => e.Comment1)
                    .IsRequired()
                    .HasColumnName("comment")
                    .HasColumnType("text")
                    .HasComment("ข้อความคอมเม้นต์")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.ImageStatus)
                    .HasColumnName("image_status")
                    .HasColumnType("int(4)")
                    .HasComment("สถานะการอัพโหลดรูปภาพ [0=ไม่มีการอัพโหลดรูปภาพ,1=มีการอัพโหลดรูปภาพ]");

                entity.Property(e => e.RefId)
                    .IsRequired()
                    .HasColumnName("ref_id")
                    .HasColumnType("varchar(150)")
                    .HasComment(@"รหัสอ้างอิงต้นทางของสื่อ เช่น food_id, res_id เป็นต้น
")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SystemName)
                    .IsRequired()
                    .HasColumnName("system_name")
                    .HasColumnType("varchar(100)")
                    .HasComment("ช่องทางระบบที่คอมเม้นต์ [food, restaurant]")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสผู้ใช้งาน");
            });

            modelBuilder.Entity<FoodCenter>(entity =>
            {
                entity.HasKey(e => e.FoodId)
                    .HasName("PRIMARY");

                entity.ToTable("food_center");

                entity.HasComment("ตารางเก็บข้อมูลอาหารส่วนกลาง");

                entity.HasIndex(e => e.Code)
                    .HasName("food_ctr_code_idx");

                entity.HasIndex(e => e.CountryId)
                    .HasName("food_ctr_country_id_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("food_ctr_created_date_idx");

                entity.HasIndex(e => e.CultureId)
                    .HasName("food_ctr_culture_id_idx");

                entity.HasIndex(e => e.LegendStatus)
                    .HasName("food_ctr_legend_status_idx");

                entity.HasIndex(e => e.NameTh)
                    .HasName("food_ctr_name_th_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("food_ctr_user_id_idx");

                entity.Property(e => e.FoodId)
                    .HasColumnName("food_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("varchar(100)")
                    .HasComment("รหัสอ้างอิงอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CookingFood)
                    .HasColumnName("cooking_food")
                    .HasColumnType("text")
                    .HasComment("วิธีการปรุง")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CountryId)
                    .HasColumnName("country_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสประเทศ");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.CultureId)
                    .HasColumnName("culture_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสวัฒนธรรมอาหาร");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasComment("คำอธิบาย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DieteticFood)
                    .HasColumnName("dietetic_food")
                    .HasColumnType("text")
                    .HasComment("โภชนาการอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Ingredient)
                    .HasColumnName("ingredient")
                    .HasColumnType("text")
                    .HasComment("วัตถุดิบ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LegendStatus)
                    .HasColumnName("legend_status")
                    .HasColumnType("int(4)")
                    .HasComment("สถานะตำนาน [0=ไม่มีตำนาน, 1=มีตำนาน]");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasColumnType("varchar(255)")
                    .HasComment("ชื่ออาหารภาษาอังกฤษ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NameTh)
                    .IsRequired()
                    .HasColumnName("name_th")
                    .HasColumnType("varchar(255)")
                    .HasComment("ชื่ออาหารภาษาไทย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปประจำตัวอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสผู้สร้างรายการ");

                entity.Property(e => e.Viewer)
                    .HasColumnName("viewer")
                    .HasColumnType("int(11)")
                    .HasComment("ยอดคนดู");
            });

            modelBuilder.Entity<FoodCountry>(entity =>
            {
                entity.HasKey(e => e.CountryId)
                    .HasName("PRIMARY");

                entity.ToTable("food_country");

                entity.HasComment("ตารางเก็บข้อมูลประเทศของอาหาร");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("food_ct_created_date_idx");

                entity.HasIndex(e => e.NameEn)
                    .HasName("food_ct_name_en_idx");

                entity.HasIndex(e => e.NameTh)
                    .HasName("food_ct_name_th_idx");

                entity.Property(e => e.CountryId)
                    .HasColumnName("country_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสประเทศ");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasComment("คำอธิบาย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasColumnType("varchar(255)")
                    .HasComment("ชื่อประเทศภาษาอังกฤษ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NameTh)
                    .IsRequired()
                    .HasColumnName("name_th")
                    .HasColumnType("varchar(255)")
                    .HasComment("ชื่อประเทศภาษาไทย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปประจำตัวของประเทศ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสผู้โพสต์");
            });

            modelBuilder.Entity<FoodCulture>(entity =>
            {
                entity.HasKey(e => e.CultureId)
                    .HasName("PRIMARY");

                entity.ToTable("food_culture");

                entity.HasComment("ตารางเก็บข้อมูลวัฒนธรรมอาหาร");

                entity.HasIndex(e => e.CountryId)
                    .HasName("food_ct_country_id_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("food_ct_created_date_idx");

                entity.HasIndex(e => e.NameTh)
                    .HasName("food_ct_name_th_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("food_ct_user_id_idx");

                entity.Property(e => e.CultureId)
                    .HasColumnName("culture_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสประเทศ");

                entity.Property(e => e.CountryId)
                    .HasColumnName("country_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสประเทศของอาหาร");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasComment("คำอธิบาย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasColumnType("varchar(255)")
                    .HasComment("ชื่อวัฒนธรรมอาหารภาษาอังกฤษ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NameTh)
                    .IsRequired()
                    .HasColumnName("name_th")
                    .HasColumnType("varchar(255)")
                    .HasComment("ชื่อวัฒนธรรมอาหารภาษาไทย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปประจำตัวของประเทศ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสผู้ใช้งาน");
            });

            modelBuilder.Entity<FoodIngredient>(entity =>
            {
                entity.ToTable("food_ingredient");

                entity.HasComment("ตารางเก็บข้อมูลวัตถุดิบอาหารส่วนกลาง");

                entity.HasIndex(e => e.Code)
                    .HasName("food_in_code_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("food_in_created_date_idx");

                entity.HasIndex(e => e.FoodId)
                    .HasName("food_in_food_id_idx");

                entity.HasIndex(e => e.LegendStatus)
                    .HasName("food_in_legend_status_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเอกลักษณ์วัตถุดิบ");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("varchar(100)")
                    .HasComment("รหัสอ้างอิงวัตถุดิบ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasComment("คำอธิบาย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FoodId)
                    .HasColumnName("food_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสอาหารส่วนกลาง");

                entity.Property(e => e.LegendStatus)
                    .HasColumnName("legend_status")
                    .HasColumnType("int(4)")
                    .HasComment("สถานะตำนาน [0=ไม่มีตำนาน, 1=มีตำนาน]");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปภาพประจำตัววัตถุดิบ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasColumnName("unit")
                    .HasColumnType("varchar(100)")
                    .HasComment("หน่วยนับ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.Weight)
                    .IsRequired()
                    .HasColumnName("weight")
                    .HasColumnType("varchar(100)")
                    .HasComment("จำนวน / น้ำหนัก")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<HistorySearch>(entity =>
            {
                entity.ToTable("history_search");

                entity.HasComment("ตารางเก็บข้อมูลการค้นหา");

                entity.HasIndex(e => e.ClientId)
                    .HasName("hs_client_id_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("hs_created_date_idx");

                entity.HasIndex(e => e.Deleted)
                    .HasName("hs_deleted_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเอกลักษณ์การค้นหา");

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasColumnName("client_id")
                    .HasColumnType("varchar(100)")
                    .HasComment("รหัสอ้างอิงตัวตนของผู้ใช้งาน")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Condition)
                    .IsRequired()
                    .HasColumnName("condition")
                    .HasColumnType("text")
                    .HasComment("กำหนดเงื่อนไขการค้นหาเป็น json")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Deleted)
                    .HasColumnName("deleted")
                    .HasColumnType("int(4)")
                    .HasComment("สถานะการลบข้อมูล [0=ไม่ลบ,1=ลบข้อมูล]");

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasColumnName("device_id")
                    .HasColumnType("varchar(150)")
                    .HasComment("รหัสอ้างอิงอุปกรณ์")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SearchText)
                    .IsRequired()
                    .HasColumnName("search_text")
                    .HasColumnType("text")
                    .HasComment("คำค้นหา")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");
            });

            modelBuilder.Entity<Legend>(entity =>
            {
                entity.ToTable("legend");

                entity.HasComment("ตารางเก็บข้อมูลตำนานของอาหารและวัตถุดิบ");

                entity.HasIndex(e => e.Code)
                    .HasName("l_code_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("l_created_date_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเอกลักษณ์ของตำนาน");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("varchar(100)")
                    .HasComment(@"รหัสอ้างอิงของรายการที่มีตำนาน
เช่น food_center.code, food_ingredient.code, restaurant_menu.code, restaurant_ingredient.code เป็นต้น
")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasComment("คำอธิบาย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LegendType)
                    .HasColumnName("legend_type")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'")
                    .HasComment("ประเภทของตำนาน 1=อาหารส่วนกลาง,2=อาหารของร้านอาหาร");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปภาพประจำ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");
            });

            modelBuilder.Entity<Media>(entity =>
            {
                entity.ToTable("media");

                entity.HasComment("ตารางเก็บข้อมูลไฟล์สื่อ");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("media_create_date_dix");

                entity.HasIndex(e => e.Filename)
                    .HasName("media_filename_idx");

                entity.HasIndex(e => e.RefId)
                    .HasName("media_ref_id_idx");

                entity.HasIndex(e => e.SystemName)
                    .HasName("media_system_name_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเอกลักษณ์ไฟล์สื่อ");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Filename)
                    .IsRequired()
                    .HasColumnName("filename")
                    .HasColumnType("varchar(255)")
                    .HasComment("ชื่อแฟ้ม-เอกสาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnName("path")
                    .HasColumnType("varchar(255)")
                    .HasComment("ตำแหน่งจัดเก็บ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RefId)
                    .IsRequired()
                    .HasColumnName("ref_id")
                    .HasColumnType("varchar(150)")
                    .HasComment("รหัสอ้างอิงต้นทางของสื่อ เช่น food_id, res_id เป็นต้น\\n")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.SystemName)
                    .IsRequired()
                    .HasColumnName("system_name")
                    .HasColumnType("varchar(100)")
                    .HasComment(@"ช่องทางระบบที่อัพโหลด [food, country, culture, food_ingredient, legend, restaurant, restaurant_menu, restaurant_ingredient, user, promotion, comment]
")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("promotion");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("p_created_date_idx");

                entity.HasIndex(e => e.Flag)
                    .HasName("p_flag_ifx");

                entity.HasIndex(e => e.ResId)
                    .HasName("p_res_id_idx");

                entity.HasIndex(e => new { e.StartDate, e.EndDate })
                    .HasName("p_start_end_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสโปรโมชัน");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(45)")
                    .HasComment("คำอธิบาย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สิ้นสุดโปรโมชัน");

                entity.Property(e => e.Flag)
                    .HasColumnName("flag")
                    .HasColumnType("int(4)")
                    .HasComment("ปักตึงโปรโมชันให้แสดง [0=ซ่อน,1=ใช้วันที่เริ่มต้น-สิ้นสุด,2=แสดงตลอดจนกว่าจะนำออก]");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)")
                    .HasComment("หัวข้อโปรโมชัน")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ResId)
                    .HasColumnName("res_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสร้านอาหาร");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่เริ่มต้นโปรโมชัน");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปภาพประจำ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.Viewer)
                    .HasColumnName("viewer")
                    .HasColumnType("int(11)")
                    .HasComment("ยอดคนดู");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.HasKey(e => e.ResId)
                    .HasName("PRIMARY");

                entity.ToTable("restaurant");

                entity.HasComment("ตารางเก็บข้อมูลร้านอาหาร");

                entity.HasIndex(e => e.CountryId)
                    .HasName("res_country_id_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("res_created_date_idx");

                entity.HasIndex(e => e.Name)
                    .HasName("res_name_idx");

                entity.HasIndex(e => e.Viewer)
                    .HasName("res_viewer_idx");

                entity.Property(e => e.ResId)
                    .HasColumnName("res_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสร้านอาหาร");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasColumnType("text")
                    .HasComment("ที่อยู่ร้านอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CarPark)
                    .HasColumnName("car_park")
                    .HasColumnType("int(4)")
                    .HasComment("มีที่จอดรถ [0=ไม่มี, 1=มี]");

                entity.Property(e => e.CountryId)
                    .HasColumnName("country_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสประเทศ");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Facebook)
                    .HasColumnName("facebook")
                    .HasColumnType("text")
                    .HasComment("ช่องทางติดต่อเฟสบุ๊ค")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("double(20,6)")
                    .HasComment("พิกัดละดิจูด");

                entity.Property(e => e.Line)
                    .HasColumnName("line")
                    .HasColumnType("text")
                    .HasComment("ช่องทางติดต่อไลน์")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("double(20,6)")
                    .HasComment("พิกัดลองจิจูด");

                entity.Property(e => e.Map)
                    .HasColumnName("map")
                    .HasColumnType("text")
                    .HasComment("แผนที่ google map")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)")
                    .HasComment("ชื่อร้านอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OpenTime)
                    .HasColumnName("open_time")
                    .HasColumnType("text")
                    .HasComment("วันเวลาเปิดปิด")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OwnerId)
                    .HasColumnName("owner_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเจ้าของร้าน");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasColumnType("varchar(255)")
                    .HasComment("เบอร์โทรศัพท์")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Province)
                    .IsRequired()
                    .HasColumnName("province")
                    .HasColumnType("varchar(100)")
                    .HasComment("จังหวัดของร้านอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Tags)
                    .HasColumnName("tags")
                    .HasColumnType("text")
                    .HasComment("แปะป้ายสำหรับค้นหา")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปประจำตัวร้านอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสผู้ใช้งาน");

                entity.Property(e => e.Video)
                    .HasColumnName("video")
                    .HasColumnType("text")
                    .HasComment("Link video youtube")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Viewer)
                    .HasColumnName("viewer")
                    .HasColumnType("int(11)")
                    .HasComment("ยอดคนดู");

                entity.Property(e => e.Website)
                    .HasColumnName("website")
                    .HasColumnType("text")
                    .HasComment("ช่องทางติดต่อเว็บไซต์")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<RestaurantIngredient>(entity =>
            {
                entity.ToTable("restaurant_ingredient");

                entity.HasComment("ตารางเก็บข้อมูลวัตถุดิบของเมนูร้านอาหาร");

                entity.HasIndex(e => e.Code)
                    .HasName("res_in_code_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("res_in_created_date_idx");

                entity.HasIndex(e => e.LegendStatus)
                    .HasName("res_in_legend_status_idx");

                entity.HasIndex(e => e.MenuId)
                    .HasName("res_in_menu_id_idx");

                entity.HasIndex(e => e.Unit)
                    .HasName("res_in_unit_idx");

                entity.HasIndex(e => e.Weight)
                    .HasName("res_in_weight_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเอกลักษณ์วัตถุดิบของร้านอาหาร");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("varchar(100)")
                    .HasComment("รหัสอ้างอิงวัตถุดิบของร้านอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasComment("คำอธิบาย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LegendStatus)
                    .HasColumnName("legend_status")
                    .HasColumnType("int(4)")
                    .HasComment("สถานะตำนาน [0=ไม่มีตำนาน, 1=มีตำนาน]");

                entity.Property(e => e.MenuId)
                    .HasColumnName("menu_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเมนูอาหาร");

                entity.Property(e => e.ResId)
                    .HasColumnName("res_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสร้านอาหาร");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปภาพประจำตัววัตถุดิบ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasColumnName("unit")
                    .HasColumnType("varchar(100)")
                    .HasComment("หน่วยนับ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.Weight)
                    .IsRequired()
                    .HasColumnName("weight")
                    .HasColumnType("varchar(100)")
                    .HasComment("จำนวน / น้ำหนัก")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<RestaurantMenu>(entity =>
            {
                entity.HasKey(e => e.MenuId)
                    .HasName("PRIMARY");

                entity.ToTable("restaurant_menu");

                entity.HasComment("ตารางเก็บข้อมูลเมนูร้านอาหาร");

                entity.HasIndex(e => e.Code)
                    .HasName("res_m_code_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("res_m_created_date_idx");

                entity.HasIndex(e => e.NameTh)
                    .HasName("res_m_name_th_idx");

                entity.HasIndex(e => e.Price)
                    .HasName("res_m_price_idx");

                entity.HasIndex(e => e.ResId)
                    .HasName("res_m_res_id_idx");

                entity.HasIndex(e => e.Viewer)
                    .HasName("res_m_viewer_idx");

                entity.Property(e => e.MenuId)
                    .HasColumnName("menu_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเมนูอาหาร");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("varchar(100)")
                    .HasComment("รหัสอ้างอิงเมนูร้านอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CookingFood)
                    .HasColumnName("cooking_food")
                    .HasColumnType("text")
                    .HasComment("วิธีการปรุง")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CountryId)
                    .HasColumnName("country_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสประเทศ");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.CultureId)
                    .HasColumnName("culture_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสวัฒนธรรมอาหาร");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasComment("คำอธิบาย")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DieteticFood)
                    .HasColumnName("dietetic_food")
                    .HasColumnType("text")
                    .HasComment("โภชนาการอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LegendStatus)
                    .HasColumnName("legend_status")
                    .HasColumnType("int(4)")
                    .HasComment("สถานะตำนาน [0=ไม่มีตำนาน, 1=มีตำนาน]");

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasColumnType("varchar(255)")
                    .HasComment("ชื่อเมนูภาษาอังกฤษ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NameTh)
                    .IsRequired()
                    .HasColumnName("name_th")
                    .HasColumnType("varchar(255)")
                    .HasComment(@"
ชื่อเมนูภาษาไทย
")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("double(20,2)")
                    .HasComment("ราคาอาหาร");

                entity.Property(e => e.ResId)
                    .HasColumnName("res_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสร้านอาหาร");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปประจำตัวร้านอาหาร")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.Viewer)
                    .HasColumnName("viewer")
                    .HasColumnType("int(11)")
                    .HasComment("ยอดคนดู");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasComment("ตารางผู้ใช้งานระบบ");

                entity.HasIndex(e => e.ClientId)
                    .HasName("client_id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("user_created_date_idx");

                entity.HasIndex(e => e.Email)
                    .HasName("email_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Phone)
                    .HasName("user_phone_idx");

                entity.HasIndex(e => e.Role)
                    .HasName("user_role_idx");

                entity.HasIndex(e => e.Username)
                    .HasName("user_username_idx");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสผู้ใช้งาน");

                entity.Property(e => e.Avatar)
                    .HasColumnName("avatar")
                    .HasColumnType("varchar(255)")
                    .HasComment("รูปประจำตัว")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasColumnName("client_id")
                    .HasColumnType("varchar(100)")
                    .HasComment("รหัสอ้างอิงตัวตน GUID")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(150)")
                    .HasComment("อีเมล")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(100)")
                    .HasComment("ชื่อจริง")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(100)")
                    .HasComment("นามสกุล")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentId)
                    .HasColumnName("parent_id")
                    .HasColumnType("int(11)")
                    .HasComment("PARENT_ID สำหรับพนักงานร้านอาหาร (OWNER PARENT_ID จะเท่ากับ NULL ส่วน STAFF PARENT_ID จะเท่ากับเจ้าของร้านคนๆนั้น)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(150)")
                    .HasComment("รหัสผ่าน")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasColumnType("varchar(50)")
                    .HasComment("เบอร์โทรศัพท์")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnName("role")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'client'")
                    .HasComment(@"สถานะ [admin, owner,staff,client]
")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(150)")
                    .HasComment("ชื่อผู้ใช้งาน")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.ToTable("user_device");

                entity.HasComment("ตารางเก็บข้อมูลอุปกรณ์");

                entity.HasIndex(e => e.ClientId)
                    .HasName("user_device_client_id_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("user_device_created_date_idx");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("user_device_device_id_idx");

                entity.HasIndex(e => e.DeviceType)
                    .HasName("user_device_device_type_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเอกลักษณ์ตารางเก็บข้อมูลอุปกรณ์");

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasColumnName("client_id")
                    .HasColumnType("varchar(100)")
                    .HasComment("รหัสอ้างอิงตัวตน GUID")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasColumnName("device_id")
                    .HasColumnType("varchar(100)")
                    .HasComment("รหัสอุปกรณ์")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DeviceType)
                    .IsRequired()
                    .HasColumnName("device_type")
                    .HasColumnType("varchar(50)")
                    .HasComment("ประเภทอุปกรณ์ [desktop,mobile,ipad]")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Expired)
                    .HasColumnName("expired")
                    .HasColumnType("timestamp")
                    .HasComment("วันเวลาที่หมดอายุ");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasColumnType("text")
                    .HasComment("รหัสยืนยันตัวตน")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");
            });

            modelBuilder.Entity<Vote>(entity =>
            {
                entity.ToTable("vote");

                entity.HasComment("ตารางเก็บข้อมูลการให้คะแนนเนื้อหา");

                entity.HasIndex(e => e.ClientId)
                    .HasName("vote_client_id_idx");

                entity.HasIndex(e => e.CreatedDate)
                    .HasName("vote_created_date_idx");

                entity.HasIndex(e => e.RefId)
                    .HasName("vote_ref_id_idx");

                entity.HasIndex(e => e.SystemName)
                    .HasName("vote_system_name_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasComment("รหัสเอกลักษณ์การให้คะแนน");

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasColumnName("client_id")
                    .HasColumnType("varchar(100)")
                    .HasComment(@"รหัสอ้างอิงตัวตนของผู้ใช้งาน
")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasComment("วันที่สร้างรายการ");

                entity.Property(e => e.RefId)
                    .IsRequired()
                    .HasColumnName("ref_id")
                    .HasColumnType("varchar(100)")
                    .HasComment("รหัสอ้างอิงต้นทางของสื่อ เช่น food_id, res_id เป็นต้น")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasColumnType("int(4)")
                    .HasComment("เกณฑ์การให้คะแนนเป็นเลขจำนวนเต็ม 1 ถึง 5 คะแนน");

                entity.Property(e => e.SystemName)
                    .IsRequired()
                    .HasColumnName("system_name")
                    .HasColumnType("varchar(100)")
                    .HasComment(@"ช่องทางระบบที่คอมเม้นต์ [food, restaurant]
")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("วันที่แก้ไขรายการ");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
