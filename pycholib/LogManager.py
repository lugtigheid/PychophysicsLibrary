#  LogManager.py
#  
#  Copyright 2013 Matthew Cutone <mdc@HP-G61-Notebook-PC>
#  

import os
import sys
import datetime

import string

# TODO: Should open a write to a file when started

class LoggingComponent(object):

    def __init__(self, OutFilePath, ExpName='Dummy Experiment', ExpVer=1.0, ExpDateTime='05/05/2009 14:10:09', 
                 SubjName='abc', SubjExtraInfo='', CondVals={0: "INFRONT", 1: "BEHIND"}, StimVals=[0.01, 0.02, 0.05, 0.1], 
                 Intervals='2AFC', RepCount=1, TrialCount=100, HeaderTemplateFile=None, 
                 Verbose=False, NewLineChar=None, CommentChar='#'):
                     
        ''' Contructor for logging object '''
        
        self._OutFilePath = OutFilePath
        self._OutFile = None

        self._ExpName = ExpName
        self._ExpVer = ExpVer
        self._ExpDateTime = ExpDateTime
        self._SubjName = SubjName
        self._SubjExtraInfo = SubjExtraInfo
        self._CondVals = CondVals
        self._StimVals = StimVals
        self._Intervals = Intervals
        self._RepCount = RepCount
        self._TrialCount = TrialCount
        
        self._Verbose = Verbose

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
        
        self._CommentChar = str(CommentChar)
        
        # TODO: check if file exists
        
        if HeaderTemplateFile != None:
            self._HeaderTemplateFile = HeaderTemplateFile
        else:
            self._HeaderTemplateFile = "default_header.txt"
        
        self._started = False
    
    def Start(self, StartDate):
        ''' Start logging and write the header '''
        
        if not self._started:
            
            header_file = open(self._HeaderTemplateFile, 'r')
            header_template = string.Template(header_file.read())
            header_file.close()
            
            brk_line = '='*60
            
            # TODO: Use platform specific newline char in header, requires
            # a replace function looking for '\n' and swapping it out for
            # self._new_line_char
            
            hdat = header_template.substitute(exp_name=self._ExpName, 
                                              exp_ver=self._ExpVer, 
                                              exp_datetime=self._ExpDateTime, 
                                              subject_name=self._SubjName, 
                                              subject_details=self._SubjExtraInfo, 
                                              cond_count=len(self._CondVals.keys()),
                                              cond_val=self._CondVals, 
                                              stim_vals=self._StimVals, 
                                              intervals=self._Intervals,
                                              rep_count=self._RepCount, 
                                              trial_count=self._TrialCount,
                                              sep_line=brk_line)
            
            if self._Verbose: print(hdat)
            
            # flag this log object at started
            self._OutFile = open(self._OutFilePath, 'a')
            self._started = True
            
            self._OutFile.write(hdat)
            self._OutFile.write(self._new_line_char)
            
        else:
            
            print("ERROR: This log object is already started.")
        
    def NewEntry(self, nTrial, Condition='', StimVal='', Response='', RT='', Extra=''):
        
        if self._started:
            
            new_entry = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}{6}".format(nTrial, Condition, 
                StimVal, Response, RT, Extra, self._new_line_char)
            if self._Verbose: print(new_entry)
            
            self._OutFile.write(new_entry)
            
        else:
            
            print("ERROR: Can not add entry, log object has not be started yet.")
        
    def Stop(self, StopDate):
        
        if self._started:
            
            # stop date should be in a nice format handled elsewhere
            self._OutFile.write(self._new_line_char)
            self._OutFile.write("# {0}: Experiment ended{1}".format(StopDate, self._new_line_char))
            self._started = False
            
            # TODO: close log file here
            self._OutFile.close()
            self._OutFile = None # set to none
        
        else:
            
            print("ERROR: This log object has not been started.")

    def __del__(self):
        # we still have a file open, something wrong occured and we have to try
        # to close the file
        if self._OutFile != None:
            warn_msg = "WARNING: LogManager has closed unexpectedly, either the program ended without calling 'Stop()' or something bad occured."
            self._OutFile.write(self._new_line_char)
            self._OutFile.write(warn_msg)
            self._OutFile.write(self._new_line_char)
            self._OutFile.close()
            
            print(warn_msg)

def main():
    # test driver
    #test_log = LoggingComponent('logging_test.txt')
    #test_log.Start(datetime.datetime.now())
    #test_log.NewEntry(nTrial=0, Condition=1, StimVal=0.5, Response=1, RT='', Extra='')
    #test_log.Stop(datetime.datetime.now())
    
    return 0
    
if __name__ == "__main__":
    main()
