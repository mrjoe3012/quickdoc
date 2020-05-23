
#@qdfunction(Gets the input*record)
def get_input():
    s = ""
    i = 0

    s = input("enter a string")
    i = int(input("enter an int"))

    r = record(i, s)

    return r

#@qdfunction(Prints the record*void)
def print_record(record):
    print("some int:", record.someint)
    print("some string:", record.somestring)

#@qdclass(Stores data)
class record:
    def __init__(self, someint, somestring):
        #@qdmfield(My field*int)
        self.someint = someint
        #@qdmfield(My other field*string)
        self.somestring = somestring
#@qdend

rec = None

rec = get_input()
print_record(rec)

