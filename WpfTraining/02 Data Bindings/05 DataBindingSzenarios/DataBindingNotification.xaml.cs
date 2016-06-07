using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Samples
{
	public partial class DataBindingNotification : Page
	{
		private List<Person> personList = new List<Person>();
		private ObservableCollection<Person> personObservableList = new ObservableCollection<Person>();

		public DataBindingNotification()
		{
			InitializeComponent();

			var newPerson = new Person();
			newPerson.FirstName = "Rainer";
			newPerson.LastName = "Stropek";
			personList.Add(newPerson);

			newPerson = new Person();
			newPerson.FirstName = "Karin";
			newPerson.LastName = "Huber";
			personObservableList.Add(newPerson);

			ListListbox.ItemsSource = personList;
			ObservableListListbox.ItemsSource = personObservableList;

			AddPersonListListbox.Click += new RoutedEventHandler(AddPersonListListbox_Click);
			AddPersonObservableListListbox.Click += new RoutedEventHandler(AddPersonObservableListListbox_Click);

			ChangePerson.Click += new RoutedEventHandler(ChangePerson_Click);
			ChangeNotifyingPerson.Click += new RoutedEventHandler(ChangeNotifyingPerson_Click);
		}

		void ChangeNotifyingPerson_Click(object sender, RoutedEventArgs e)
		{
			var person = FindResource("NotifyingPerson") as NotifyingPerson;
			person.FirstName = person.FirstName + " 2";
			person.LastName = person.LastName + " 2";
		}

		void ChangePerson_Click(object sender, RoutedEventArgs e)
		{
			var person = FindResource("Person") as Person;
			person.FirstName = person.FirstName + " 2";
			person.LastName = person.LastName + " 2";
		}

		void AddPersonObservableListListbox_Click(object sender, RoutedEventArgs e)
		{
			var newPerson = new Person();
			newPerson.FirstName = "Rainer";
			newPerson.LastName = "Stropek";
			personObservableList.Add(newPerson);
		}

		void AddPersonListListbox_Click(object sender, RoutedEventArgs e)
		{
			var newPerson = new Person();
			newPerson.FirstName = "Karin";
			newPerson.LastName = "Huber";
			personList.Add(newPerson);
		}

	}
}