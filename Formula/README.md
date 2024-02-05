```
Author:     Cheuk Yin Lau
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Laconsyn
Repo:       https://github.com/Laconsyn/cs3500-spring24-spreadsheet-Laconsyn
Date:       4-Feb-2024 23:00 pm
Project:    Formula
Copyright:  CS 3500 and Cheuk Yin Lau - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:

The Formula object worked properly and stands on its own. There are no bugs or deficiencies by now. 
There are no exception expected to be thrown. Wrong input could result in Formula Format exceptions or Formula Errors. 
No extension libraries are used currently. 
 Both 'e' and 'E' are acceptable for scietific notation input. The conversion occurs before normalization. 
 Syntax and restrictions are checked in private methods that could change in the future. 
The exception class and error structure is not modified. 

# Assignment Specific Topics
Question: You will notice that the starter file contains multiple classes and structs. 
Is this a good idea from a software engineering point of view?
Yes. The developer could understand the requirement and restrictions easily from the starter file. 
It is also more efficient by focusing the development on an individual portion of work, rather than working from scratch. 

Question: If a Formula is Immutable (cannot change) does this mean it will always evaluate to the same value?  
What if the formula is changed in a cell at a later point, what happens?
No. The evaluation value could be changed by giving different lookup methods. 
Changing the formula in a cell could construct a new formula object with updated formula. 

Question: How do you make a class immutable?
To make a class immutable, I could make all fields private or readonly. The methods should be only getting value without
modifying anything in the class. 
In the formula class, All fields are private, and all methods does not change any values in the class. 

Question: How are you storing your formula and thus how can you compare two formulas to see if they are the same?
The formula are stored using a list of tokens in order. 
The formulas are compared by using the ToString() method. Since identical formulas would result in same string output, 
using the ToString() method could compare two formulas and check their differences. 

# Consulted Peers:

I didn't discuss about the tests. I have a relatively clear understanding on what to do. 

# References:

Starter Code given in Assignment Two