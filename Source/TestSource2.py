#@qdclass(Object representing a person)
class person:
	def __init__(self, name, age):
		#@qdmfield(The name of the person*string)
		self.name = name
		#@qdmfield(The age of the person*int)
		self.age = age
	
	#@qdmfunction(Make the person older*void)
	#@qdparam(years*How many years older the person should be*int)
	def get_older(self, years):
		self.age += years
#@qdend

#@qdfunction(Gets a person from user*person)
def get_input():
	age = int(input("Enter the age: "))
	name input("Enter the name: ")
	p = person(name, age)
	return p
	
#@qdfunction(Prints a person*void)
#@qdparam(person*The person to print*person)
def print_person(person):
	print("Name:", person.name)
	print("Age:", person.age)
	

my_person = get_input()

my_person.get_older(5)

print_person(my_person)
