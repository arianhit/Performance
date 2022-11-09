using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using LumenWorks.Framework.IO.Csv;
using System.IO;

namespace Performance
{
    class Program
    {

        static void Main(string[] args)
        {

            //G:/My Drive/University Sheffieeld Hallam/Y1/Software Project/Performance/Performance/CSV files/New_Students.csv
            User user = new User();
            Student student = new Student();
            Course course = new Course();
            Result result = new Result();
            Module module = new Module();
            Lecture lecture = new Lecture();
            Lecturer lecturer = new Lecturer();
            Admin admin = new Admin();
            SuperAdmin superAdmin = new SuperAdmin();
            List<int> blockStudentIds = new List<int>();
            List<int> StudentIdsForAdmin = new List<int>();
            List<string> putPerformanceAudit = new List<string>();
            string auditAdminPath = "G:/My Drive/University Sheffieeld Hallam/Y1/Software Project/Performance/Performance/CSV files/Audit_Admins.csv";

            DateTime Now = DateTime.Now;

            //we need an sql connection variable


            SqlConnection connection = Database();
            UserMenu();


            void UserMenu()
            {

                int userChosenNumber = 0;
                bool inUserMenu = true;
                bool inLecturerMarkMenu = true;
                bool inLecturerModuleMenu = true;
                bool inLecturerStudentIdInput = true;
                bool inAdminActionMenu = true;
                bool inAdminPerformanceMenu = true;
                bool adminStudentInput = true;
                bool inSuperAdminActionMenu = true;
                bool studentUnBlockMenu = true;

                while (inUserMenu == true)
                {
                    while (userChosenNumber < 1 || userChosenNumber > 4)
                    {
                        Console.WriteLine("Which user are you?(enter number)\n");
                        Console.WriteLine("1.student ");
                        Console.WriteLine("2.lecturer ");
                        Console.WriteLine("3.admin ");
                        Console.WriteLine("4.super admin ");
                        userChosenNumber = Convert.ToInt32(Console.ReadLine());
                        if (userChosenNumber < 1 || userChosenNumber > 4)
                        {
                            Console.Clear();
                            Console.WriteLine("****Plese chose number between 1 to 4*****\n\n");
                        }

                    }

                    switch (userChosenNumber)
                    {
                        case 1:

                            user.type = "Student";

                            logIn();

                            StudentPerformance();
                            void StudentPerformance()
                            {

                            }
                            if (user.validLogin == true)
                            {
                                try
                                {


                                    string queryRead = "SELECT * FROM Student WHERE User_Id = '" + user.Id + "' ";
                                    SqlCommand read = new SqlCommand(queryRead, connection);
                                    SqlDataReader dtreader = read.ExecuteReader();
                                    dtreader.Read();
                                    student.Id = Convert.ToInt32(dtreader.GetValue(0).ToString());
                                    student.CourseId = Convert.ToInt32(dtreader.GetValue(1).ToString());
                                    student.firstName = dtreader.GetValue(2).ToString();
                                    student.lastName = dtreader.GetValue(3).ToString();
                                    student.abcentHours = Convert.ToSingle(dtreader.GetValue(5).ToString());
                                    student.performance = dtreader.GetValue(6).ToString();
                                    student.status = dtreader.GetValue(7).ToString();



                                    if (student.status.ToUpper() == "BLOCK")
                                    {

                                        Console.WriteLine("You are blocked student for " + dtreader.GetValue(8).ToString() + "reason! ");
                                        dtreader.Close();
                                        inUserMenu = false;

                                    }
                                    else
                                    {
                                        dtreader.Close();
                                        if (student.status.ToUpper() == "NEW")
                                        {
                                            Console.WriteLine("Welcome " + student.firstName + "");
                                            Console.WriteLine("As you are logging in for first time you need to set a password for yourself please Enter your new password: ");
                                            string newPassword = Console.ReadLine();
                                            while (true)
                                            {
                                                Console.WriteLine("Please confirm your password");
                                                string confPassword = Console.ReadLine();
                                                if (newPassword == confPassword)
                                                {
                                                    string queryUpdatePassword = "UPDATE Users SET UserPassword = '" + confPassword + "' WHERE User_Id = '" + user.Id + "'";
                                                    SqlCommand updpass = new SqlCommand(queryUpdatePassword, connection);
                                                    updpass.ExecuteNonQuery();

                                                    string queryUpdateStudentst = "UPDATE Student SET Student_Status = 'unblock' WHERE Student_Id = '" + student.Id + "'";
                                                    SqlCommand updstst = new SqlCommand(queryUpdateStudentst, connection);
                                                    updstst.ExecuteNonQuery();
                                                    break;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("!Wrong confirmation\n");
                                                }
                                            }

                                        }
                                        AttendanceCalculater();
                                        Console.WriteLine("\n\n" + student.lastName + student.firstName + " Performance : \n");
                                        Console.WriteLine("Attendence : " + student.attendance.ToString("%" + "0.000") + " \n Total hours: " + course.totalHours + "  Present hours: " + student.presentHours + "  Abcent hours: " + student.abcentHours + "\n\n");
                                        Console.WriteLine("Marks : \n\n");

                                        moduleMarkGeter();

                                        Console.WriteLine("Overall Mark :%" + result.overallMark + "\n");
                                        float overallMarkIncAt = (result.overallMark + (student.attendance * 100)) / 2;
                                        if (overallMarkIncAt > 0 && overallMarkIncAt < 39)
                                        {
                                            student.performance = "Insufficient";
                                        }
                                        else if (overallMarkIncAt > 40 && overallMarkIncAt < 49)
                                        {
                                            student.performance = "Sufficient";

                                        }
                                        else if (overallMarkIncAt > 50 && overallMarkIncAt < 59)
                                        {
                                            student.performance = "Good";

                                        }
                                        else if (overallMarkIncAt > 60 && overallMarkIncAt < 69)
                                        {
                                            student.performance = "Very Good";

                                        }
                                        else if (overallMarkIncAt > 70 && overallMarkIncAt < 84)
                                        {
                                            student.performance = "Excelent";

                                        }
                                        else if (overallMarkIncAt > 84 && overallMarkIncAt < 100)
                                        {
                                            student.performance = "Perfect";

                                        }
                                        else
                                        {
                                            student.performance = "Invalid";
                                        }

                                        string queryUpdateStudent = "UPDATE Student SET Student_Performance = '" + student.performance + "' WHERE Student_Id = '" + student.Id + "'";
                                        SqlCommand updstu = new SqlCommand(queryUpdateStudent, connection);
                                        updstu.ExecuteNonQuery();

                                        Console.WriteLine("Overall resualt(Including Attendance):%" + overallMarkIncAt.ToString("0.00") + "");
                                        Console.WriteLine("You are an*" + student.performance + "* Student , " + student.firstName + "");
                                        inUserMenu = false;
                                    }
                                    string queryUpdatePerformance = "UPDATE Student SET Student_Performance = '" + student.performance + "' WHERE Student_Id = '" + student.Id + "'";
                                    SqlCommand updStPer = new SqlCommand(queryUpdatePerformance, connection);
                                    updStPer.ExecuteNonQuery();
                                }

                                catch (SqlException x)
                                {
                                    Console.WriteLine(x.Message);
                                }
                            }

                            break;
                        case 2:



                            user.type = "Lecturer";

                            inLecturerModuleMenu = true;
                            logIn();
                            LectureGradeEditor();
                            void LectureGradeEditor()
                            {
                            }
                            if (user.validLogin == true)
                            {
                                while (inLecturerModuleMenu == true)
                                {
                                    try
                                    {
                                        inLecturerStudentIdInput = true;
                                        string queryReadLecturer = "SELECT * FROM Lecturer WHERE User_Id = '" + user.Id + "' ";
                                        SqlCommand ReadLecturer = new SqlCommand(queryReadLecturer, connection);
                                        SqlDataReader dReadLecturer = ReadLecturer.ExecuteReader();
                                        dReadLecturer.Read();
                                        lecturer.Id = Convert.ToInt32(dReadLecturer.GetValue(0).ToString());
                                        lecturer.firstName = dReadLecturer.GetValue(1).ToString();
                                        lecturer.lastName = dReadLecturer.GetValue(2).ToString();
                                        dReadLecturer.Close();

                                        Console.WriteLine("Hello , " + lecturer.firstName + " chose your module :(For exit enter 0)");

                                        List<string> moduleNames = new List<string>();
                                        List<int> ModuleIds = new List<int>();
                                        string queryReadModuleId = "SELECT * FROM Lecture WHERE Lecturer_Id = '" + lecturer.Id + "' ";
                                        SqlCommand ReadModuleId = new SqlCommand(queryReadModuleId, connection);
                                        SqlDataReader dReadModuleId = ReadModuleId.ExecuteReader();
                                        while (dReadModuleId.Read())
                                        {
                                            ModuleIds.Add(Convert.ToInt32(dReadModuleId.GetValue(2).ToString()));
                                        }
                                        dReadModuleId.Close();
                                        List<int> moduleIds = ModuleIds.Distinct().ToList();
                                        foreach (int moduleID in moduleIds)
                                        {
                                            string queryReadModule = "SELECT * FROM Module WHERE Module_Id = '" + moduleID + "' ";

                                            SqlCommand ReadModule = new SqlCommand(queryReadModule, connection);
                                            SqlDataReader dReadModule = ReadModule.ExecuteReader();
                                            while (dReadModule.Read())
                                            {

                                                Console.WriteLine("     " + moduleID + ". " + dReadModule.GetValue(1).ToString() + " ");
                                            }
                                            dReadModule.Close();
                                        }
                                        List<int> StudentIds = new List<int>();
                                        int lecturerModuleNum = Convert.ToInt32(Console.ReadLine());
                                        if (lecturerModuleNum == 0)
                                        {
                                            inLecturerModuleMenu = false;
                                            inUserMenu = false;
                                            inLecturerStudentIdInput = false;
                                        }
                                        string queryReadStudent = "SELECT * FROM Resault WHERE Module_Id = '" + lecturerModuleNum + "' ";
                                        SqlCommand ReadStudent = new SqlCommand(queryReadStudent, connection);
                                        SqlDataReader dReadStudent = ReadStudent.ExecuteReader();
                                        while (dReadStudent.Read())
                                        {
                                            StudentIds.Add(Convert.ToInt32(dReadStudent.GetValue(4).ToString()));
                                        }
                                        dReadStudent.Close();

                                        while (inLecturerStudentIdInput == true)
                                        {

                                            Console.WriteLine("Now please enter the student ID of your student in the module ," + lecturer.firstName + "(for Exit enter 0)");
                                            int lecturerStudentNum = Convert.ToInt32(Console.ReadLine());
                                            if (StudentIds.Contains(lecturerStudentNum))
                                            {
                                                string queryRead = "SELECT * FROM Student WHERE Student_Id = '" + lecturerStudentNum + "' ";
                                                SqlCommand read = new SqlCommand(queryRead, connection);
                                                SqlDataReader dtreader = read.ExecuteReader();
                                                dtreader.Read();
                                                student.Id = Convert.ToInt32(dtreader.GetValue(0).ToString());
                                                student.CourseId = Convert.ToInt32(dtreader.GetValue(1).ToString());
                                                student.firstName = dtreader.GetValue(2).ToString();
                                                student.lastName = dtreader.GetValue(3).ToString();
                                                student.abcentHours = Convert.ToSingle(dtreader.GetValue(5).ToString());
                                                student.performance = dtreader.GetValue(6).ToString();
                                                dtreader.Close();
                                                Console.WriteLine("\n\n" + student.lastName + student.firstName + " Markes : \n");

                                                moduleMarkGeter();
                                                int lecturerChosenNumberForExamMark = 0;

                                                while (inLecturerMarkMenu == true)
                                                {
                                                    inLecturerMarkMenu = true;
                                                    Console.WriteLine("         Which mark do you want to edit :\n ");
                                                    Console.WriteLine("         1.Coursework");
                                                    Console.WriteLine("         2.Final exam ");
                                                    Console.WriteLine("         3.Exit\n\n");
                                                    lecturerChosenNumberForExamMark = Convert.ToInt32(Console.ReadLine());
                                                    if (lecturerChosenNumberForExamMark < 1 || lecturerChosenNumberForExamMark > 3)
                                                    {

                                                        Console.WriteLine("****Plese chose number between 1 to 3*****\n\n");
                                                    }
                                                    else
                                                    {
                                                        switch (lecturerChosenNumberForExamMark)
                                                        {
                                                            case 1:
                                                                Console.WriteLine("Now please enter the CourseWork mark of " + student.firstName + "  " + student.lastName + " in  Module that you chose ," + lecturer.firstName + "");

                                                                int lecturerNewFinalExamMark = 0;
                                                                while (lecturerNewFinalExamMark > 100 || lecturerNewFinalExamMark <= 0)
                                                                {
                                                                    lecturerNewFinalExamMark = Convert.ToInt32(Console.ReadLine());
                                                                }

                                                                string queryReadPrevFE = "SELECT * FROM Resault WHERE (Module_Id = '" + lecturerModuleNum + "') AND (Student_Id = '" + lecturerStudentNum + "')";
                                                                SqlCommand ReadPrevFE = new SqlCommand(queryReadPrevFE, connection);
                                                                SqlDataReader dReadPrevFE = ReadPrevFE.ExecuteReader();
                                                                dReadPrevFE.Read();
                                                                int prevFE = Convert.ToInt32(dReadPrevFE.GetValue(2).ToString());
                                                                dReadPrevFE.Close();

                                                                string queryUpdateFinalExam = "UPDATE Resault SET Final_Exam_Mark = '" + lecturerNewFinalExamMark + "' WHERE (Module_Id = '" + lecturerModuleNum + "') AND (Student_Id = '" + lecturerStudentNum + "')";
                                                                SqlCommand updFE = new SqlCommand(queryUpdateFinalExam, connection);
                                                                updFE.ExecuteNonQuery();
                                                                Console.WriteLine("CourseWork mark changed to : " + lecturerNewFinalExamMark + " from " + prevFE + " succsefully !");

                                                                moduleMarkGeter();

                                                                break;


                                                            case 2:

                                                                Console.WriteLine("Now please enter the Final exam mark of " + student.firstName + "  " + student.lastName + " in  Module that you chose ," + lecturer.firstName + "");


                                                                int lecturerNewCoursworkMark = 0;

                                                                while (lecturerNewCoursworkMark > 100 || lecturerNewCoursworkMark <= 0)
                                                                {
                                                                    lecturerNewCoursworkMark = Convert.ToInt32(Console.ReadLine());
                                                                }
                                                                string queryReadPrevCW = "SELECT * FROM Resault WHERE (Module_Id = '" + lecturerModuleNum + "') AND (Student_Id = '" + lecturerStudentNum + "')";
                                                                SqlCommand ReadPrevCW = new SqlCommand(queryReadPrevCW, connection);
                                                                SqlDataReader dReadPrevCW = ReadPrevCW.ExecuteReader();
                                                                dReadPrevCW.Read();
                                                                int prevCW = Convert.ToInt32(dReadPrevCW.GetValue(3).ToString());
                                                                dReadPrevCW.Close();

                                                                string queryUpdateCourseWork = "UPDATE Resault SET Course_Work_Mark = '" + lecturerNewCoursworkMark + "' WHERE (Module_Id = '" + lecturerModuleNum + "') AND (Student_Id = '" + lecturerStudentNum + "')";
                                                                SqlCommand updCW = new SqlCommand(queryUpdateCourseWork, connection);
                                                                updCW.ExecuteNonQuery();
                                                                Console.WriteLine("CourseWork mark changed to : " + lecturerNewCoursworkMark + " from  " + prevCW + "  succsefully !");

                                                                moduleMarkGeter();

                                                                break;
                                                            case 3:
                                                                inLecturerMarkMenu = false;
                                                                break;
                                                        }

                                                    }
                                                }

                                            }
                                            else if (lecturerStudentNum == 0)
                                            {
                                                inLecturerStudentIdInput = false;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Student Not Found Please Try Again :");
                                                lecturerStudentNum = Convert.ToInt32(Console.ReadLine());
                                            }
                                        }



                                    }
                                    catch (SqlException x)
                                    {

                                        Console.WriteLine(x.Message);
                                        break;
                                    }

                                }

                            }
                            break;
                        case 3:

                            user.type = "Admin";

                            List<int> StudentIdsForAdmin = new List<int>();
                            logIn();
                            AdminPerformanceEditor();
                            void AdminPerformanceEditor(){

                            }
                            if (user.validLogin == true)
                            {
                                while (inAdminActionMenu == true)
                                {
                                    inAdminActionMenu = true;
                                    try
                                    {
                                        string queryReadAdmin = "SELECT * FROM Admin WHERE User_Id = '" + user.Id + "'";
                                        SqlCommand ReadAdmin = new SqlCommand(queryReadAdmin, connection);
                                        SqlDataReader dReadAdmin = ReadAdmin.ExecuteReader();
                                        dReadAdmin.Read();
                                        admin.firstName = dReadAdmin.GetValue(2).ToString();
                                        admin.lastName = dReadAdmin.GetValue(3).ToString();
                                        admin.Id = Convert.ToInt32(dReadAdmin.GetValue(0).ToString());
                                        dReadAdmin.Close();
                                    }
                                    catch (SqlException x)
                                    {

                                        Console.WriteLine(x.Message);
                                        break;
                                    }
                                    Console.WriteLine("Hello , " + admin.firstName + " chose your action : ");
                                    Console.WriteLine("\t1.Edit or put performance ");
                                    Console.WriteLine("\t2.View all student of the course ");
                                    Console.WriteLine("\t3.Import new Students");
                                    Console.WriteLine("\t4.block student");
                                    Console.WriteLine("\t5.Exit\n");
                                    int adminActionNum = Convert.ToInt32(Console.ReadLine());
                                    if (adminActionNum < 1 || adminActionNum > 5)
                                    {

                                        Console.WriteLine("****Plese chose number between 1 to 5*****\n\n");
                                    }
                                    else
                                    {
                                        switch (adminActionNum)

                                        {

                                            case 1:
                                                while (adminStudentInput == true)
                                                {
                                                    adminStudentInput = true;
                                                    Console.WriteLine("\tOk " + admin.firstName + " Enter the student ID of student you want edit:(enter 0 to exit) ");
                                                    int adminStudentId = Convert.ToInt32(Console.ReadLine());

                                                    if (adminStudentId == 0)
                                                    {
                                                        adminStudentInput = false;
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {

                                                            string queryReadStudent = "SELECT * FROM Student ";
                                                            SqlCommand ReadStudent = new SqlCommand(queryReadStudent, connection);
                                                            SqlDataReader dReadStudent = ReadStudent.ExecuteReader();
                                                            while (dReadStudent.Read())
                                                            {
                                                                StudentIdsForAdmin.Add(Convert.ToInt32(dReadStudent.GetValue(0).ToString()));
                                                            }
                                                            dReadStudent.Close();
                                                        }
                                                        catch (SqlException x)
                                                        {
                                                            Console.WriteLine(x.Message);
                                                        }
                                                        if (StudentIdsForAdmin.Contains(adminStudentId))
                                                        {

                                                            File.AppendAllText(auditAdminPath, admin.firstName + "," + admin.lastName + "," + admin.Id + "," + Now + "," + adminStudentId + "," + "Edit or put performance" + "\n");
                                                            try
                                                            {
                                                                string queryRead = "SELECT * FROM Student WHERE Student_Id = '" + adminStudentId + "' ";
                                                                SqlCommand read = new SqlCommand(queryRead, connection);
                                                                SqlDataReader dtreader = read.ExecuteReader();
                                                                dtreader.Read();
                                                                student.CourseId = Convert.ToInt32(dtreader.GetValue(1).ToString());
                                                                student.Id = Convert.ToInt32(dtreader.GetValue(0).ToString());
                                                                student.CourseId = Convert.ToInt32(dtreader.GetValue(1).ToString());
                                                                student.firstName = dtreader.GetValue(2).ToString();
                                                                student.lastName = dtreader.GetValue(3).ToString();
                                                                student.abcentHours = Convert.ToSingle(dtreader.GetValue(5).ToString());
                                                                student.performance = dtreader.GetValue(6).ToString();
                                                                dtreader.Close();
                                                            }
                                                            catch (SqlException x)
                                                            {
                                                                Console.WriteLine(x.Message);
                                                            }

                                                            AttendanceCalculater();
                                                            Console.WriteLine("\n\n" + student.lastName + student.firstName + " Performance : \n");
                                                            Console.WriteLine("Attendence : " + student.attendance.ToString("%" + "0.000") + " \n Total hours: " + course.totalHours + "  Present hours: " + student.presentHours + "  Abcent hours: " + student.abcentHours + "\n\n");
                                                            Console.WriteLine("Marks : \n\n");

                                                            moduleMarkGeter();

                                                            Console.WriteLine("Overall Mark :%" + result.overallMark + "\n");
                                                            float overallMarkIncAt = (result.overallMark + (student.attendance * 100)) / 2;
                                                            Console.WriteLine("Overall resualt(Including Attendance):%" + overallMarkIncAt.ToString("0.00") + "\n\n");


                                                            CourseOverallGrade(course.Id);
                                                            Console.WriteLine("Overall score in the student course:" + course.courseOverallGrade + "");
                                                            Console.WriteLine("Overall Final exam score in the student course:" + course.courseOverallGradeInFE + "");
                                                            Console.WriteLine("Overall coursework score in the student course:" + course.courseOverallGradeInCW + "");

                                                            Console.WriteLine("He is *" + student.performance + "* Student");
                                                            inAdminPerformanceMenu = true;
                                                            while (inAdminPerformanceMenu == true)
                                                            {
                                                                Console.WriteLine("***************Edit Performance Menu *****************\n\n\n ");
                                                                Console.WriteLine("         Which Performance do you want to put :\n ");
                                                                Console.WriteLine("         1.Insufficient (0-39)");
                                                                Console.WriteLine("         2.Sufficient (40-49) ");
                                                                Console.WriteLine("         3.Good(50 - 59) ");
                                                                Console.WriteLine("         4.Very Good (60-69) ");
                                                                Console.WriteLine("         5.Excellent(70-84) ");
                                                                Console.WriteLine("         6.Perfect(85+)");
                                                                Console.WriteLine("         7.Exit\n\n");
                                                                string prevStudentPerformance = student.performance;
                                                                int adminChosenNumberForEditPerformance = Convert.ToInt32(Console.ReadLine());
                                                                if (adminChosenNumberForEditPerformance < 1 || adminChosenNumberForEditPerformance > 7)
                                                                {

                                                                    Console.WriteLine("****Plese chose number between 1 to 7*****\n\n");
                                                                }
                                                                else
                                                                {
                                                                    switch (adminChosenNumberForEditPerformance)
                                                                    {
                                                                        case 1:

                                                                            student.performance = "Insufficient";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + "succsesfully\n");


                                                                            break;
                                                                        case 2:
                                                                            student.performance = "Sufficient";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 3:
                                                                            student.performance = "Good";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 4:
                                                                            student.performance = "Very Good";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 5:
                                                                            student.performance = "Excelent";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 6:
                                                                            student.performance = "Perfect";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 7:
                                                                            inAdminPerformanceMenu = false;
                                                                            break;
                                                                    }
                                                                    string queryUpdatePerformance = "UPDATE Student SET Student_Performance = '" + student.performance + "' WHERE Student_Id = '" + student.Id + "'";
                                                                    SqlCommand updStPer = new SqlCommand(queryUpdatePerformance, connection);
                                                                    updStPer.ExecuteNonQuery();
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Student Not Found Please Try Again (For return enter 0):");
                                                            adminStudentId = Convert.ToInt32(Console.ReadLine());
                                                        }

                                                        inAdminActionMenu = false;


                                                    }


                                                }

                                                break;

                                            case 2:

                                                ViewAllStudent();
                                                File.AppendAllText(auditAdminPath, admin.firstName + "," + admin.lastName + "," + admin.Id + "," + Now + "," + admin.adminCourseIdViewAll + "," + "View all student of the course" + "\n");

                                                break;

                                            case 3:

                                                ImportStudents();
                                                string numberOfImportedStudentWithS = student.count + "student(s)";
                                                File.AppendAllText(auditAdminPath, admin.firstName + "," + admin.lastName + "," + admin.Id + "," + Now + "," + numberOfImportedStudentWithS + "," + "Imort new student" + "\n");

                                                break;

                                            case 4:

                                                BlockStudent();
                                                File.AppendAllText(auditAdminPath, admin.firstName + "," + admin.lastName + "," + admin.Id + "," + Now + "," + student.Id + "," + "Block Student" + "\n");


                                                break;

                                            case 5:

                                                inAdminActionMenu = false;

                                                break;
                                        }
                                    }

                                }

                                inUserMenu = false;
                            }



                            break;
                        case 4:
                            user.type = "SuperAdmin";

                            List<int> StudentIdsForSupAdmin = new List<int>();
                            logIn();
                            SuperAdminPerformanceEditor();
                            void SuperAdminPerformanceEditor()
                            {

                            }
                            if (user.validLogin == true)
                            {
                                while (inSuperAdminActionMenu == true)
                                {
                                    inSuperAdminActionMenu = true;
                                    try
                                    {
                                        string queryReadSuperAdmin = "SELECT * FROM Super_Admin WHERE User_Id = '" + user.Id + "'";
                                        SqlCommand ReadSuperAdmin = new SqlCommand(queryReadSuperAdmin, connection);
                                        SqlDataReader dReadSuperAdmin = ReadSuperAdmin.ExecuteReader();
                                        dReadSuperAdmin.Read();
                                        superAdmin.firstName = dReadSuperAdmin.GetValue(2).ToString();
                                        dReadSuperAdmin.Close();
                                    }
                                    catch (SqlException x)
                                    {

                                        Console.WriteLine(x.Message);
                                        break;
                                    }
                                    Console.WriteLine("Hello , " + admin.firstName + " chose your action : ");
                                    Console.WriteLine("\t1.Edit or put performance ");
                                    Console.WriteLine("\t2.View all student of the course ");
                                    Console.WriteLine("\t3.Import new Students ");
                                    Console.WriteLine("\t4.block student");
                                    Console.WriteLine("\t5.unblock student");
                                    Console.WriteLine("\t6.Audit Admins");
                                    Console.WriteLine("\t7.Exit\n");
                                    int superAdminActionNum = Convert.ToInt32(Console.ReadLine());
                                    if (superAdminActionNum < 1 || superAdminActionNum > 7)
                                    {

                                        Console.WriteLine("****Plese chose number between 1 to 5*****\n\n");
                                    }
                                    else
                                    {
                                        switch (superAdminActionNum)

                                        {

                                            case 1:
                                                adminStudentInput = true;
                                                while (adminStudentInput == true)
                                                {
                                                    Console.WriteLine("\tOk " + superAdmin.firstName + " Enter the student ID of student you want edit:(enter 0 to exit) ");
                                                    int superAdminStudentId = Convert.ToInt32(Console.ReadLine());
                                                    if (superAdminStudentId == 0)
                                                    {
                                                        adminStudentInput = false;
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {

                                                            string queryReadStudent = "SELECT * FROM Student ";
                                                            SqlCommand ReadStudent = new SqlCommand(queryReadStudent, connection);
                                                            SqlDataReader dReadStudent = ReadStudent.ExecuteReader();
                                                            while (dReadStudent.Read())
                                                            {
                                                                StudentIdsForSupAdmin.Add(Convert.ToInt32(dReadStudent.GetValue(0).ToString()));
                                                            }
                                                            dReadStudent.Close();
                                                        }
                                                        catch (SqlException x)
                                                        {
                                                            Console.WriteLine(x.Message);
                                                        }
                                                        if (StudentIdsForSupAdmin.Contains(superAdminStudentId))
                                                        {
                                                            try
                                                            {
                                                                string queryRead = "SELECT * FROM Student WHERE Student_Id = '" + superAdminStudentId + "' ";
                                                                SqlCommand read = new SqlCommand(queryRead, connection);
                                                                SqlDataReader dtreader = read.ExecuteReader();
                                                                dtreader.Read();
                                                                student.CourseId = Convert.ToInt32(dtreader.GetValue(1).ToString());
                                                                student.Id = Convert.ToInt32(dtreader.GetValue(0).ToString());
                                                                student.CourseId = Convert.ToInt32(dtreader.GetValue(1).ToString());
                                                                student.firstName = dtreader.GetValue(3).ToString();
                                                                student.lastName = dtreader.GetValue(4).ToString();
                                                                student.abcentHours = Convert.ToSingle(dtreader.GetValue(6).ToString());
                                                                student.performance = dtreader.GetValue(7).ToString();
                                                                dtreader.Close();
                                                            }
                                                            catch (SqlException x)
                                                            {
                                                                Console.WriteLine(x.Message);
                                                            }

                                                            AttendanceCalculater();
                                                            Console.WriteLine("\n\n" + student.lastName + student.firstName + " Performance : \n");
                                                            Console.WriteLine("Attendence : " + student.attendance.ToString("%" + "0.000") + " \n Total hours: " + course.totalHours + "  Present hours: " + student.presentHours + "  Abcent hours: " + student.abcentHours + "\n\n");
                                                            Console.WriteLine("Marks : \n\n");
                                                            Console.WriteLine("\tModule   \t\t\tCoursework\t\tFinalexam\t\tOverall\n");
                                                            moduleMarkGeter();

                                                            Console.WriteLine("Overall Mark :%" + result.overallMark + "\n");
                                                            float overallMarkIncAt = (result.overallMark + (student.attendance * 100)) / 2;
                                                            Console.WriteLine("Overall resualt(Including Attendance):%" + overallMarkIncAt.ToString("0.00") + "\n\n");


                                                            CourseOverallGrade(course.Id);
                                                            Console.WriteLine("Overall score in the student course:" + course.courseOverallGrade + "");
                                                            Console.WriteLine("Overall Final exam score in the student course:" + course.courseOverallGradeInFE + "");
                                                            Console.WriteLine("Overall coursework score in the student course:" + course.courseOverallGradeInCW + "");

                                                            Console.WriteLine("He is *" + student.performance + "* Student");
                                                            inAdminPerformanceMenu = true;
                                                            while (inAdminPerformanceMenu == true)
                                                            {
                                                                Console.WriteLine("***************Edit Performance Menu *****************\n\n\n ");
                                                                Console.WriteLine("         Which mark do you want to edit :\n ");
                                                                Console.WriteLine("         1.Insufficient (0-39)");
                                                                Console.WriteLine("         2.Sufficient (40-49) ");
                                                                Console.WriteLine("         3.Good(50 - 59) ");
                                                                Console.WriteLine("         4.Very Good (60-69) ");
                                                                Console.WriteLine("         5.Excelent(70-84 ");
                                                                Console.WriteLine("         6.Perfect(85+) (40-49) ");
                                                                Console.WriteLine("         7.Exit\n\n");
                                                                string prevStudentPerformance = student.performance;
                                                                int adminChosenNumberForEditPerformance = Convert.ToInt32(Console.ReadLine());
                                                                if (adminChosenNumberForEditPerformance < 1 || adminChosenNumberForEditPerformance > 7)
                                                                {

                                                                    Console.WriteLine("****Plese chose number between 1 to 7*****\n\n");
                                                                }
                                                                else
                                                                {
                                                                    switch (adminChosenNumberForEditPerformance)
                                                                    {
                                                                        case 1:

                                                                            student.performance = "Insufficient";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 2:
                                                                            student.performance = "Sufficient";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 3:
                                                                            student.performance = "Good";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 4:
                                                                            student.performance = "Very Good";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 5:
                                                                            student.performance = "Excelent";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 6:
                                                                            student.performance = "Perfect";
                                                                            Console.WriteLine("Performance changed from " + prevStudentPerformance + " to " + student.performance + " succsesfully\n");

                                                                            break;
                                                                        case 7:
                                                                            inAdminPerformanceMenu = false;
                                                                            break;
                                                                    }

                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Student Not Found Please Try Again (For return enter 0):");

                                                        }

                                                        inSuperAdminActionMenu = false;


                                                    }


                                                }
                                                break;

                                            case 2:
                                                ViewAllStudent();
                                                break;

                                            case 3:
                                                ImportStudents();
                                                break;
                                            case 4:
                                                BlockStudent();
                                                break;
                                            case 5:
                                                UnBlockStudent();
                                                void UnBlockStudent()
                                                {
                                                    while (studentUnBlockMenu == true)
                                                    {
                                                        studentUnBlockMenu = true;
                                                        Console.WriteLine("Please Enter the Student Id of the student you want to unblock(Enter 0 for exit)");
                                                        int adminEnteredStudentId = Convert.ToInt32(Console.ReadLine());
                                                        List<int> studentIdsForAdmin = new List<int>();
                                                        try
                                                        {

                                                            string queryReadStudent = "SELECT * FROM Student ";
                                                            SqlCommand ReadStudent = new SqlCommand(queryReadStudent, connection);
                                                            SqlDataReader dReadStudent = ReadStudent.ExecuteReader();
                                                            while (dReadStudent.Read())
                                                            {
                                                                studentIdsForAdmin.Add(Convert.ToInt32(dReadStudent.GetValue(0).ToString()));
                                                            }
                                                            dReadStudent.Close();
                                                        }
                                                        catch (SqlException x)
                                                        {
                                                            Console.WriteLine(x.Message);
                                                        }


                                                        if (studentIdsForAdmin.Contains(adminEnteredStudentId))
                                                        {

                                                            string queryReadStudentForadminToBlock = "SELECT * FROM Student WHERE Student_Id = " + adminEnteredStudentId + "";
                                                            SqlCommand ReadStudentForadminToBlock = new SqlCommand(queryReadStudentForadminToBlock, connection);
                                                            SqlDataReader dReadStudentForadminToBlock = ReadStudentForadminToBlock.ExecuteReader();
                                                            dReadStudentForadminToBlock.Read();
                                                            student.firstName = dReadStudentForadminToBlock.GetValue(2).ToString();
                                                            student.lastName = dReadStudentForadminToBlock.GetValue(3).ToString();
                                                            student.status = dReadStudentForadminToBlock.GetValue(7).ToString();
                                                            student.Rstatus = dReadStudentForadminToBlock.GetValue(8).ToString();
                                                            dReadStudentForadminToBlock.Close();
                                                            if (student.status.ToUpper() == "BLOCK")
                                                            {
                                                                while (true)
                                                                {
                                                                    Console.WriteLine("Do you want to unblock " + student.firstName + "  " + student.lastName + "?(Y yes/N No)");
                                                                    string adminEnteredBlockConfirm = Console.ReadLine();

                                                                    if (adminEnteredBlockConfirm.ToUpper() == "Y" || adminEnteredBlockConfirm.ToUpper() == "YES")
                                                                    {
                                                                        Console.WriteLine("Enter the summary reason like(missunderstanding)");
                                                                        string adminEnteredBlockReason = Console.ReadLine();
                                                                        student.status = "unblock";
                                                                        student.Rstatus = adminEnteredBlockReason;
                                                                        string queryUpdateStudentStatus = "UPDATE Student SET Student_Status = '" + student.status + "' WHERE Student_Id = '" + adminEnteredStudentId + "'";
                                                                        SqlCommand updSS = new SqlCommand(queryUpdateStudentStatus, connection);
                                                                        updSS.ExecuteNonQuery();
                                                                        string queryUpdateStudentResStatus = "UPDATE Student SET Student_Reason_Of_Status = '" + student.Rstatus + "' WHERE Student_Id = '" + adminEnteredStudentId + "'";
                                                                        SqlCommand updSRS = new SqlCommand(queryUpdateStudentResStatus, connection);
                                                                        updSRS.ExecuteNonQuery();
                                                                        Console.WriteLine("" + student.firstName + " " + student.lastName + " Has been unblock becuase of " + student.Rstatus + " Sucsesfully \n");
                                                                        break;
                                                                    }
                                                                    else if (adminEnteredBlockConfirm.ToUpper() == "N" || adminEnteredBlockConfirm.ToUpper() == "NO")
                                                                    {

                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("PLEASE ENTER Y OR N");
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Student is unblock");
                                                            }

                                                        }
                                                        else if (adminEnteredStudentId == 0)
                                                        {

                                                            break;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Student Not Found Please Try Again (For return enter 0) :");

                                                        }

                                                    }



                                                }

                                                break;
                                            case 6:
                                                AuditAdmin();
                                                void AuditAdmin()
                                                {
                                                    List<string> adminData = File.ReadAllLines("G:/My Drive/University Sheffieeld Hallam/Y1/Software Project/Performance/Performance/CSV files/Audit_Admins.csv").ToList();
                                                    Console.WriteLine("First Name" + "\t" + "Last Name" + "\t" + "Id" + "\t\t" + "Date & Time" + "\t\t" + "student/Course ID" + "\t" + "Action");
                                                    Console.WriteLine("\n--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                                                    foreach (string data in adminData)
                                                    {

                                                        Console.WriteLine(data.Replace(",", "\t\t") + "\n--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                                                    }
                                                }
                                                break;
                                            case 7:
                                                inSuperAdminActionMenu = false;
                                                break;
                                        }
                                    }

                                }

                                inUserMenu = false;
                            }



                            break;
                    }

                }
            }
            void logIn()
            {

                while (user.numberOfTrials < 4)
                {
                    string userPsswordFromDB = "";
                    Console.WriteLine("*-*-*-*-*-*-*-*-*    Log in Menu   *-*-*-*-*-*-*-*-*\n");
                    Console.WriteLine("Enter your user name :");
                    user.UserName = Console.ReadLine();


                    Console.WriteLine("Enter your password :");
                    string userPassWord = Console.ReadLine();
                    try
                    {
                        List<string> userNames = new List<string>();
                        string queryReadUsernames = "SELECT * FROM Users WHERE UserType = '" + user.type + "'";
                        SqlCommand ReadUsernames = new SqlCommand(queryReadUsernames, connection);
                        SqlDataReader dReadUsernames = ReadUsernames.ExecuteReader();
                        while (dReadUsernames.Read())
                        {
                            userNames.Add(dReadUsernames.GetValue(1).ToString());
                        }
                        dReadUsernames.Close();
                        if (userNames.Contains(user.UserName))
                        {
                            string queryRead2 = "SELECT * FROM Users WHERE UserName ='" + user.UserName + "' ";

                            SqlCommand read2 = new SqlCommand(queryRead2, connection);
                            SqlDataReader dtreader2 = read2.ExecuteReader();
                            dtreader2.Read();
                            user.Id = Convert.ToInt32(dtreader2.GetValue(0).ToString());
                            userPsswordFromDB = dtreader2.GetValue(2).ToString();

                            dtreader2.Close();
                        }
                        if (userPassWord == userPsswordFromDB)
                        {
                            user.validLogin = true;
                            user.numberOfTrials = 5;
                        }
                        else
                        {
                            user.validLogin = false;
                            switch (user.numberOfTrials)
                            {
                                case 0:
                                    Console.WriteLine("****************ACCES DENIED WRONG USERNAME OR PASSWORD************** \nTRY AGAIN\nYOU HAVE JUST *3* ATTEMPTS\n");
                                    user.numberOfTrials++;
                                    break;
                                case 1:
                                    Console.WriteLine("****************ACCES DENIED WRONG USERNAME OR PASSWORD************** \nTRY AGAIN\nYOU HAVE JUST *2* ATTEMPTS\n");
                                    user.numberOfTrials++;
                                    break;
                                case 2:
                                    Console.WriteLine("****************ACCES DENIED WRONG USERNAME OR PASSWORD************** \nTRY AGAIN\n THIS IS YOUR LAST ATTEPT BE CAEFULE!!\n");
                                    user.numberOfTrials++;
                                    break;
                                case 3:
                                    Console.WriteLine("****************ACCES DENIED WRONG USERNAME OR PASSWORD************** \nYOU HAD 3 ATTEMPTS AND YOU DO NOT ALLOW LOGIN AGAIN\n");
                                    user.numberOfTrials++;
                                    break;
                            }
                        }
                    }
                    catch (SqlException x)
                    {

                        Console.WriteLine(x.Message);

                    }

                }

            }
            void AttendanceCalculater()
            {
                string queryReadCourseTH = "SELECT * FROM Course WHERE Course_Id = '" + student.CourseId + "' ";
                SqlCommand ReadCourseTH = new SqlCommand(queryReadCourseTH, connection);
                SqlDataReader dReadCourseTH = ReadCourseTH.ExecuteReader();
                dReadCourseTH.Read();
                course.totalHours = Convert.ToSingle(dReadCourseTH.GetValue(2).ToString());
                dReadCourseTH.Close();
                student.presentHours = course.totalHours - student.abcentHours;
                student.attendance = student.presentHours / course.totalHours;
            }
            void moduleMarkGeter()
            {

                List<string> moduleNames = new List<string>();
                List<int> ModuleIds = new List<int>();
                List<int> finalExamMarks = new List<int>();
                List<int> courseWorkMarks = new List<int>();
                try
                {
                    string queryReadResult = "SELECT * FROM Resault WHERE Student_Id = '" + student.Id + "' ";
                    SqlCommand ReadResult = new SqlCommand(queryReadResult, connection);
                    SqlDataReader dReadResult = ReadResult.ExecuteReader();
                    while (dReadResult.Read())
                    {
                        finalExamMarks.Add(Convert.ToInt32(dReadResult.GetValue(2).ToString()));
                        courseWorkMarks.Add(Convert.ToInt32(dReadResult.GetValue(3).ToString()));
                        ModuleIds.Add(Convert.ToInt32(dReadResult.GetValue(1).ToString()));
                    }
                    dReadResult.Close();
                    foreach (int moduleID in ModuleIds)
                    {
                        string queryReadModule = "SELECT * FROM Module WHERE Module_Id = '" + moduleID + "' ";

                        SqlCommand ReadModule = new SqlCommand(queryReadModule, connection);
                        SqlDataReader dReadModule = ReadModule.ExecuteReader();
                        while (dReadModule.Read())
                        {
                            moduleNames.Add(dReadModule.GetValue(1).ToString());
                        }
                        dReadModule.Close();
                    }
                }
                catch (SqlException x)
                {
                    Console.WriteLine(x.Message);
                }
                Console.WriteLine("\tModule   \t\t\tCoursework\t\tFinalexam\t\tOverall\n");
                for (int numberOfModule = 0; numberOfModule < moduleNames.Count(); numberOfModule++)
                {

                    Console.WriteLine("\t" + moduleNames[numberOfModule] + "\t\t\t: " + finalExamMarks[numberOfModule] + "\t\t\t" + courseWorkMarks[numberOfModule] + "\t\t\t" + ((finalExamMarks[numberOfModule] + courseWorkMarks[numberOfModule]) / 2) + "");
                    result.totalMark = result.totalMark + ((finalExamMarks[numberOfModule] + courseWorkMarks[numberOfModule]) / 2);
                }
                result.overallMark = result.totalMark / moduleNames.Count();
                Console.WriteLine("\n");
            }
            void CourseOverallGrade(int courseId)
            {
                List<int> studentIds = new List<int>();
                string queryReadStudentIdForResult = "SELECT * FROM Student WHERE Course_Id = '" + student.CourseId + "' ";
                SqlCommand ReadStudentIdForResult = new SqlCommand(queryReadStudentIdForResult, connection);
                SqlDataReader dReadStudentIdForResult = ReadStudentIdForResult.ExecuteReader();
                while (dReadStudentIdForResult.Read())
                {
                    studentIds.Add(Convert.ToInt32(dReadStudentIdForResult.GetValue(0).ToString()));
                }
                dReadStudentIdForResult.Close();
                foreach (int studentId in studentIds)
                {
                    int numnberOfResults = 0;

                    float courseWorkMarksTotal = 0;


                    float finalExamMarksTotal = 0;


                    string queryReadResult = "SELECT * FROM Resault WHERE Student_Id = '" + studentId + "' ";

                    SqlCommand ReadResult = new SqlCommand(queryReadResult, connection);
                    SqlDataReader dReadResult = ReadResult.ExecuteReader();
                    while (dReadResult.Read())
                    {
                        numnberOfResults++;
                        courseWorkMarksTotal = courseWorkMarksTotal + Convert.ToSingle(dReadResult.GetValue(3).ToString());
                        finalExamMarksTotal = finalExamMarksTotal + Convert.ToSingle(dReadResult.GetValue(2).ToString());
                    }
                    dReadResult.Close();
                    course.courseOverallGradeInCW = courseWorkMarksTotal / numnberOfResults;
                    course.courseOverallGradeInFE = finalExamMarksTotal / numnberOfResults;
                    course.courseOverallGrade = (course.courseOverallGradeInCW + course.courseOverallGradeInFE) / 2;
                }
            }
            void BlockStudent()
            {
                while (true)
                {

                    Console.WriteLine("Please Enter the Student Id of the student you want to block(Enter 0 for exit)");
                    int adminEnteredStudentId = Convert.ToInt32(Console.ReadLine());

                    List<int> studentIdsForAdmin = new List<int>();
                    try
                    {

                        string queryReadStudent = "SELECT * FROM Student ";
                        SqlCommand ReadStudent = new SqlCommand(queryReadStudent, connection);
                        SqlDataReader dReadStudent = ReadStudent.ExecuteReader();
                        while (dReadStudent.Read())
                        {
                            studentIdsForAdmin.Add(Convert.ToInt32(dReadStudent.GetValue(0).ToString()));
                        }
                        dReadStudent.Close();
                    }
                    catch (SqlException x)
                    {
                        Console.WriteLine(x.Message);
                    }


                    if (studentIdsForAdmin.Contains(adminEnteredStudentId))
                    {
                        string queryReadStudentForadminToBlock = "SELECT * FROM Student WHERE Student_Id = " + adminEnteredStudentId + "";
                        SqlCommand ReadStudentForadminToBlock = new SqlCommand(queryReadStudentForadminToBlock, connection);
                        SqlDataReader dReadStudentForadminToBlock = ReadStudentForadminToBlock.ExecuteReader();
                        dReadStudentForadminToBlock.Read();
                        student.Id = Convert.ToInt32(dReadStudentForadminToBlock.GetValue(0).ToString());
                        student.firstName = dReadStudentForadminToBlock.GetValue(2).ToString();
                        student.lastName = dReadStudentForadminToBlock.GetValue(3).ToString();
                        student.status = dReadStudentForadminToBlock.GetValue(7).ToString();
                        student.Rstatus = dReadStudentForadminToBlock.GetValue(8).ToString();
                        dReadStudentForadminToBlock.Close();
                        if (student.status.ToUpper() == "BLOCK")
                        {
                            Console.WriteLine("Student is already block");
                        }
                        else
                        {
                            while (true)
                            {
                                Console.WriteLine("Do you want to block " + student.firstName + " " + student.lastName + " ?(Y yes/N No)");
                                string adminEnteredBlockConfirm = Console.ReadLine();

                                if (adminEnteredBlockConfirm.ToUpper() == "Y" || adminEnteredBlockConfirm.ToUpper() == "YES")
                                {
                                    Console.WriteLine("Enter the summary reason like(Invoicing)");
                                    string adminEnteredBlockReason = Console.ReadLine();
                                    student.status = "BLOCK";
                                    student.Rstatus = adminEnteredBlockReason;
                                    string queryUpdateStudentStatus = "UPDATE Student SET Student_Status = '" + student.status + "' WHERE Student_Id = '" + adminEnteredStudentId + "'";
                                    SqlCommand updSS = new SqlCommand(queryUpdateStudentStatus, connection);
                                    updSS.ExecuteNonQuery();
                                    string queryUpdateStudentResStatus = "UPDATE Student SET Student_Reason_Of_Status = '" + student.Rstatus + "' WHERE Student_Id = '" + adminEnteredStudentId + "'";
                                    SqlCommand updSRS = new SqlCommand(queryUpdateStudentResStatus, connection);
                                    updSRS.ExecuteNonQuery();
                                    Console.WriteLine("" + student.firstName + " " + student.lastName + " Has been block becuase of " + student.Rstatus + " Sucsesfully \n");
                                    break;
                                }
                                else if (adminEnteredBlockConfirm.ToUpper() == "N" || adminEnteredBlockConfirm.ToUpper() == "NO")
                                {

                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("PLEASE ENTER Y OR N");
                                }
                            }
                        }


                    }
                    else if (adminEnteredStudentId == 0)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Student Not Found Please Try Again (For return enter 0) :");

                    }

                }
            }

            void ImportStudents()
            {

                while (true)
                {
                    Console.WriteLine("Pleas Enter the file path of the CSV file that you want import : (Enter 0 for exit)");
                    string filePathOfNewStudents = Console.ReadLine();
                    Console.WriteLine("\n");

                    //"G:/My Drive/University Sheffieeld Hallam/Y1/Software Project/Performance/Performance/CSV files/New_Students.csv";
                    if (File.Exists(filePathOfNewStudents))
                    {

                        List<string> newStudentsDataInList = File.ReadAllLines(filePathOfNewStudents).ToList();

                        foreach (string data in newStudentsDataInList)
                        {
                            try
                            {
                                student.count++;
                                string qeuryInsert = "INSERT INTO Student (Student_First_Name,Student_Last_Name,Student_Date_Of_Birth,Student_Id,Student_Abcsent_Hours,Course_Id,Student_Status) VALUES (" + "'" + data.Replace(",", "','") + "'" + ")";
                                SqlCommand insert = new SqlCommand(qeuryInsert, connection);
                                insert.ExecuteNonQuery();


                            }
                            catch (SqlException x)
                            {
                                Console.WriteLine(x.Message);
                            }

                        }


                        string newStudentsData = File.ReadAllText(filePathOfNewStudents);



                        List<int> userIds = new List<int>();
                        List<string> newStudentUserNames = new List<string>();
                        List<int> newStudentIds = new List<int>();
                        List<int> newCourseIds = new List<int>();
                        List<int> newModuleIds = new List<int>();


                        string queryReadNewStudent = "SELECT * FROM Student WHERE Student_Status = 'New' ";
                        SqlCommand ReadNewStudent = new SqlCommand(queryReadNewStudent, connection);
                        SqlDataReader dReadNewStudent = ReadNewStudent.ExecuteReader();
                        while (dReadNewStudent.Read())
                        {
                            newCourseIds.Add(Convert.ToInt32(dReadNewStudent.GetValue(1).ToString()));
                            newStudentIds.Add(Convert.ToInt32(dReadNewStudent.GetValue(0).ToString()));
                            userIds.Add(Convert.ToInt32(dReadNewStudent.GetValue(9).ToString()));
                            newStudentUserNames.Add("c" + dReadNewStudent.GetValue(0).ToString());
                        }
                        dReadNewStudent.Close();

                        for (int i = 0; i < userIds.Count(); i++)
                        {
                            string qeuryInsert1 = "INSERT INTO Users (User_Id,UserName,UserPassword,UserType) VALUES ('" + userIds[i] + "','" + newStudentUserNames[i] + "','12345','Student')";
                            SqlCommand insert1 = new SqlCommand(qeuryInsert1, connection);
                            insert1.ExecuteNonQuery();
                        }
                        foreach (int cid in newCourseIds)
                        {
                            string queryReadModuleIds = "SELECT * FROM Lecture WHERE Course_Id ='" + cid + "' ";
                            SqlCommand ReadModuleIds = new SqlCommand(queryReadModuleIds, connection);
                            SqlDataReader dReadModuleIds = ReadModuleIds.ExecuteReader();
                            while (dReadModuleIds.Read())
                            {
                                newModuleIds.Add(Convert.ToInt32(dReadModuleIds.GetValue(2).ToString()));
                            }

                            dReadModuleIds.Close();
                        }

                        for (int i = 0; i < newStudentIds.Count(); i++)
                        {
                            string qeuryInsert2 = "INSERT INTO Resault (Module_Id,Final_Exam_Mark,Course_Work_Mark,Student_Id) VALUES ('" + newModuleIds[i] + "','00','00','" + newStudentIds[i] + "')";
                            SqlCommand insert2 = new SqlCommand(qeuryInsert2, connection);
                            insert2.ExecuteNonQuery();
                        }


                        Console.WriteLine("First Name" + "\t" + "Last Name" + "\t" + "Dat of Birth" + "\t" + "    Student ID" + "    " + "Abcent hours" + "\t" + "Courrse ID" + "\t" + "Student Status");
                        newStudentsData = newStudentsData.Replace(",", "\t\t");
                        Console.WriteLine(newStudentsData);
                        Console.WriteLine("\n**********************Students Imported sucssefullly***********************************\n");

                        break;
                    }
                    else if (filePathOfNewStudents == "0")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Sorry, '" + filePathOfNewStudents + "' doesn't exist.\n");
                    }


                }

            }

            void ViewAllStudent()
            {
                while (true)
                {
                    Console.WriteLine("Please Enter the Course Id of the course you want see all it's students");
                    admin.adminCourseIdViewAll = Convert.ToInt32(Console.ReadLine());

                    List<int> CourseIdsForAdmin = new List<int>();
                    string queryReadCourseIds = "SELECT * FROM Course ";
                    SqlCommand ReadCourseIds = new SqlCommand(queryReadCourseIds, connection);
                    SqlDataReader dReadCourseIds = ReadCourseIds.ExecuteReader();
                    dReadCourseIds.Read();

                    while (dReadCourseIds.Read())
                    {
                        CourseIdsForAdmin.Add(Convert.ToInt32(dReadCourseIds.GetValue(0).ToString()));
                    }

                    dReadCourseIds.Close();
                    if (CourseIdsForAdmin.Contains(admin.adminCourseIdViewAll))
                    {
                        Console.WriteLine("\tId\tFirst Name\tLast Name\tDate Of Birth\tAbcsent Hours\tStudent Performance\tStudent Status and Reason Of Status");
                        try
                        {
                            string queryReadStudentForadmin = "SELECT * FROM Student WHERE Course_Id = " + admin.adminCourseIdViewAll + "";
                            SqlCommand ReadStudentForadmin = new SqlCommand(queryReadStudentForadmin, connection);
                            SqlDataReader dReadStudentForadmin = ReadStudentForadmin.ExecuteReader();
                            while (dReadStudentForadmin.Read())
                            {
                                student.Id = Convert.ToInt32(dReadStudentForadmin.GetValue(0).ToString());
                                student.CourseId = Convert.ToInt32(dReadStudentForadmin.GetValue(1).ToString());
                                student.firstName = dReadStudentForadmin.GetValue(2).ToString();
                                student.lastName = dReadStudentForadmin.GetValue(3).ToString();
                                student.abcentHours = Convert.ToSingle(dReadStudentForadmin.GetValue(5).ToString());
                                student.dateOfBirth = dReadStudentForadmin.GetValue(4).ToString();
                                student.performance = dReadStudentForadmin.GetValue(6).ToString();
                                student.status = dReadStudentForadmin.GetValue(7).ToString();
                                student.Rstatus = dReadStudentForadmin.GetValue(8).ToString();
                                Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                                Console.WriteLine("\t" + student.Id + "\t" + student.firstName + "\t\t" + student.lastName + "\t" + student.dateOfBirth.Substring(0, 9) + "\t" + student.abcentHours + "\t\t" + student.performance + "\t\t" + student.status + "\t" + student.Rstatus + "");
                                Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n");

                            }

                            dReadStudentForadmin.Close();
                        }
                        catch (SqlException x)
                        {
                            Console.WriteLine(x.Message);
                        }
                        break;
                    }
                    else if (admin.adminCourseIdViewAll == 0)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Course Not Found! Enter the Course ID again:");
                    }
                }

            }

            SqlConnection Database()
            {


                //we need the database address as an string
                string str;
                SqlConnection con;


                try
                {
                    //database address
                    str = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename= G:\My Drive\University Sheffieeld Hallam\Y1\Software Project\Performance\Performance\ERDTabales.mdf ;Integrated Security=True";

                    //creating connection with the database address
                    con = new SqlConnection(str);

                    //open connection
                    con.Open();
                    //writing a massage that databas is connected

                    return con;

                } //catch for any error from sql
                catch (SqlException x)
                {
                    con = new SqlConnection("");
                    Console.WriteLine(x.Message);
                    return con;
                }

            }

        }
    }
}




class User
{
    public string type;
    public string UserName;
    public int Id;
    public bool validLogin;
    public int numberOfTrials;
}
class Student
{
    public string firstName;
    public string lastName;
    public int Id;
    public int CourseId;
    public float abcentHours;
    public float presentHours;
    public float attendance;
    public string dateOfBirth;
    public string performance;
    public string status;
    public string Rstatus;
    public int count;

}
class Course
{
    public float totalHours;
    public string name;
    public int Id;
    public float courseOverallGrade;
    public float courseOverallGradeInCW;
    public float courseOverallGradeInFE;

}
class Module
{
    public int moduleId;
    public string moduleName;
}
class Result
{
    public int resultId;
    public int finalExam;
    public int courseWork;
    public float totalMark;
    public float overallMark;

}
class Lecture
{
    public int Id;
    public int room;

}
class Lecturer
{
    public int Id;
    public string firstName;
    public string lastName;
}
class Admin
{
    public string firstName;
    public string lastName;
    public int Id;
    public int adminCourseIdViewAll;


}
class SuperAdmin
{
    public string firstName;

}







