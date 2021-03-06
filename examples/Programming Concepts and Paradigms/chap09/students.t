% File "students.t".
% Prepare a file of student records in binary form
% input from the keyboard and stored in file "school".
% As records are stored their position in the file is recorded
% in an array called "directory".
% Records are then accessed randomly.
type studentType :
    record
        name : string (30)
        address : string (40)
        year : int
    end record
var student : studentType
% Open file called "school" for writing and reading in binary form.
var fileNumber : int
open : fileNumber, "school", read, write, seek
assert fileNumber > 0
const maxStudents := 500
type whereAbouts :
    record
        name : string (30)
        location : int
    end record
type directoryType :
    array 1 .. maxStudents of whereAbouts
var directory : directoryType
var filePosition : int
var numberOfStudents : int

% Find position of a student's record.
% Position set to -1 if not found.
procedure find (var name : string (30), var position : int)
    % Uses global variables directory and numberOfStudents.
    if length (name) < 30 then
        % Pad with blanks.
        name := name + repeat (" ", 30 - length (name))
    end if
    position := - 1 % Set to -1 in case name not found.
    % Search list for name by linear search.
    for i : 1 .. numberOfStudents
        if name = directory (i).name then
            position := directory (i).location
            exit
        end if
    end for
end find

put "How many students are there? " ..
get numberOfStudents
put "Enter ", numberOfStudents, " student records"
put ""
% Place labels over input columns to show user where to enter data.
put "name" : 30, "address" : 40, "year"
for i : 1 .. numberOfStudents
    % Determine where next record will start in file.
    tell : fileNumber, filePosition
    get skip, student.name : 30, student.address : 40, student.year
    directory (i).name := student.name
    directory (i).location := filePosition
    write : fileNumber, student
end for
%Access the records randomly
var wantedStudent : string (30)
loop
    var place : int
    put "Enter name of student, a maximum of 30 characters"
    get skip, wantedStudent : *
    find (wantedStudent, place)
    if place not= - 1 then
        seek : fileNumber, place
        read : fileNumber, student
        put "Here is the information you want"
        put "name" : 30, "address" : 40, "year"
        put student.name : 30, student.address : 40, student.year
    else
        put "Student not in file" % If place = -1 name not there.
    end if
end loop

