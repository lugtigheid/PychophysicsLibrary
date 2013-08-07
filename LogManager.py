#  LogManager.py
#  
#  Copyright 2013 Matthew Cutone <mdc@HP-G61-Notebook-PC>
#  

import os
import sys
import datetime

import string

# default header template style

header_txt = """# Name: $name
# Started: $start
# Conditions: $cond
# Stimulus values: $stimv
#
# 0=Trail, 1=Condition, 2=Stimval, 3=Response, 4=RT, 5=Extra
"""
HEADER_TEMPLATE = string.Template(header_txt)

# TODO: Should open a write to a file when started

class LoggingComponent(object):

    def __init__(self, ExpName='test', Conditions={}, StimVals=[], Verbose=False, NewLineChar=None):
        ''' Contructor for logging object '''
        
        self._ExpName = ExpName
        self._Conditions = Conditions
        self._Stim_Vals = StimVals
        
        self._Verbose = Verbose
        
        self._log_dat = ""
        self._started = False
        
        # use the platform dependent newline character
        if NewLineChar == None:
            
            if sys.platform == 'linux2': # linux
                self._new_line_char = '\n'
            elif sys.platform == 'win32': # windows
                self._new_line_char = '\r\n'
            elif sys.platform == 'darwin': # mac osx
                self._new_line_char = '\r'
            else:
                self._new_line_char = '\n' # toaster or whatever
                
        else:
            # can be used to enforce consistency across platforms
            self._new_line_char = NewLineChar
    
    def GetLogData(self):
        return self._log_dat
    
    def Start(self, StartDate):
        ''' Start logging and write the header '''
        
        if not self._started:
            
            hdat = HEADER_TEMPLATE.substitute(name=self._ExpName, start=StartDate, 
                                              cond=self._Conditions, stimv=self._Stim_Vals)
            
            if self._Verbose: print(hdat)
            
            self._log_dat = hdat
            
            # flag this log object at started
            self._started = True
            
            # TODO: open a file for writing here
            
        else:
            
            print("ERROR: This log object is already started.")
        
    def NewEntry(self, nTrial, Condition='', StimVal='', Response='', RT='', Extra=''):
        
        if self._started:
            
            new_entry = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}".format(nTrial, Condition, 
                StimVal, Response, RT, Extra)
            if self._Verbose: print(new_entry)
            
            self._log_dat = "{0}{1}{2}".format(self._log_dat, new_entry, self._new_line_char)
            
        else:
            
            print("ERROR: Can not add entry, log object has not be started yet.")
        
    def Stop(self, StopDate):
        
        if self._started:
            
            # stop date should be in a nice format handled elsewhere
            self._log_dat = "{0}{1}{2}".format(self._log_dat, StopDate. self._new_line_char)
            self._started = False
            
            # TODO: close log file here
        
        else:
            
            print("ERROR: This log object has not been started.")

    def DumpToFile(self, file_path):
        ''' Dump the log text to a specfied file '''
        pass

def main():
    test_log = LoggingComponent()
    test_log.Start(datetime.datetime.now())
    test_log.NewEntry(nTrial=0, Condition=1, StimVal=0.5, Response=1, RT='', Extra='')
    test_log.Stop(datetime.datetime.now())
    
    return 0
    
if __name__ == "__main__":
    main()
