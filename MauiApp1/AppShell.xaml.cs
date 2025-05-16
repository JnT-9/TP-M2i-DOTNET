using MauiApp1.Views;

namespace MauiApp1;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Register routes for navigation
		Routing.RegisterRoute("taskdetail", typeof(TaskDetailPage));
		Routing.RegisterRoute("newtask", typeof(NewTaskPage));
	}
}
