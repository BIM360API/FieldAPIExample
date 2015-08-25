using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.BIM360Field.APIService;
using Autodesk.BIM360Field.APIService.Models;
using Autodesk.BIM360Field.APIService.Support;

namespace APIExample
{
    class APIExample
    {
        static void Main(string[] args)
        {
            string username = "user@company.com";
            string password = "supersecretpassword";
            string server = "https://api.velasystems.com";

            switch (args.Length)
            {
                case 0:
                case 1:
                    Console.WriteLine("You must specify a username, password and optionally the server to connect to");
                    Console.WriteLine(string.Format("APIExample.exe {0} {1} {2}", username, password, server));
                    Environment.Exit(1);
                    break;
                case 2:
                    username = args[0];
                    password = args[1];
                    break;
                case 3:
                    username = args[0];
                    password = args[1];
                    server = args[2];
                    break;
            }

            Console.WriteLine(string.Format("Connecting to BIM 360 Field API service on {0} as {1}", server, username));


            try
            {
                API api = new API(server);

                api.authenticate(username, password);

                Console.WriteLine("Authenticated successfully. Retrieving project list.\n\n");

                List<Project> projects = api.getProjects();

                Console.WriteLine("Project ID\t\t\t\tProject Name");
                Console.WriteLine("----------\t\t\t\t------------");
                foreach (Project project in projects)
                {
                    Console.WriteLine(string.Format("{0}\t{1}", project.project_id, project.name));
                }

                Console.WriteLine("Retrieving list of Checklists for first project\n\n");

                // Set the default project on the API service object and you won't have to pass it each time you make a call.
                // Just DON'T forget to change it if you need to interact with other projects!
                api.DefaultProject = projects[0];

                List<Checklist> checklists = api.getChecklists(null, null, 0, 10000); // Defaults to 25
                Console.WriteLine(string.Format("The project {0} has {1} checklist(s)", api.DefaultProject.name, checklists.Count));

                if (checklists.Count > 0)
                {
                    Console.WriteLine("Checklist ID\t\t\t\tName");
                    Console.WriteLine("------------\t\t\t\t----");

                    foreach (Checklist checklist in checklists)
                    {
                        Console.WriteLine(string.Format("{0}\t{1}", checklist.id, checklist.name));
                    }

                    Console.WriteLine("\n\n");

                    Checklist firstChecklist = api.getChecklist(checklists[0].id);
                    Console.WriteLine(string.Format("The checklist with ID {0} has {1} sections. Please inspect this object to see what else is available!", firstChecklist.id, firstChecklist.sections.Count));
                }
            }
            catch (BIM360FieldAPIException ex)
            {
                Console.WriteLine(string.Format("API service threw an exception: {0} {1}", ex.Code, ex.Message));
            }
            catch (UnauthorizedAccessException ua)
            {
                Console.WriteLine("Failed to authenticate with the supplied credentials.");
            }
        }
    }
}
