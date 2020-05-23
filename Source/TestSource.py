#@qdfield(sexy field*string)
var = "cool string"

#@qdclass(This is an object)
class myClass:
    #@qdmfield(This is an integer*int)
    myint = 0
    #@qdmfield(This is a string*string)
    mystring = ""

    def __init__(self):
        #@qdmfield(This is part of the object*int)
        self.memberInt = 0
        #@qdmfield(This is also part of the object*string)
        self.memberString = ""

    #@qdmfunction(This is a member function*void)
    #@qdparam(someParam*The parameter yo*null nigga)
    #@qdparam(otherparam*Another bro*null brudda)
    def member_function(self):
        pass

#@qdend

#@qdfunction(This method prints out the parameter.*void)
#@qdparam(data*The data to be printed*string)
def writeline(data, boolean):
    print(data)
