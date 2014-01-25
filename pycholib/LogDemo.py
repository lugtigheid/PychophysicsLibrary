#  LogDemo.py
#  

from LogManager import *

def main():
    
    # Create a logging object
    test_log = LoggingComponent('logging_test.txt')
    
    # Write a bunch of fields and accompanying data
    fields =[['Exp Name', 'JUST TESTING'], 
             ['Program', '/home/testuser/experiments/myexperiment.py'],
             ['Version', '2014-01-10'],
             ['Subject', 'mdc'],
             ['StartTime', None], # some fields can be written afterwards, given None as a placeholder
             ['Conditions', 'binocular'],
             ['Dist', 1000]]
    
    # LoggingComponent can store a header and footer. LogHeading is an object
    # that stores data to be written and a header or footer. Here we create a
    # heading object.
    
    head = LogHeading(test_log, fields, Margin=1) 
    head.UpdateFieldData('StartTime', test_log.GetTimeStamp()) # we can update a field when needed

    # Now we create a footer object, this can contain anything such as the
    # results of a data summary. For now we hust want the footer to contain a
    # stop time for the experiment.
    
    fields =[['StopTime', None]]
    foot = LogHeading(test_log, fields, UseBreak=False, Margin=1)
    
    # Set the header and footer objects.
    test_log.SetHeader(head)
    test_log.SetFooter(foot)
    
    # Set the fields to write our data to, these values become arguments for the
    # AddRecord() function after StartLogging is called. They must be valid
    # Python variable names.
    
    test_log.SetDataFields("Trial", "Cond", "Interv", "StimLevel", "Response", "Correct", "Latency")
    
    test_log.StartLogging() # start logging
    
    for i in range(10): # simulate trials
    
        # Add a new line to the data file, will be formatted automatically.
        test_log.AddRecord(Trial=i, Cond=1, Interv=1, StimLevel=0.5, Response=1, Correct=1, Latency=0.3452)
    
    # The experiment has ended update the StopTime field.
    foot.UpdateFieldData('StopTime', test_log.GetTimeStamp())
    
    test_log.StopLogging() # stop logging, prevents anything else from being written
    
    # Write the data to the file and close it.
    test_log.DumpToCSVFile(format_type='rcsv', no_comments=False)
    
    return 0

if __name__ == '__main__':
    main()

