#  LogManager.py
#  

import os
import sys
import datetime

import string

# TODO: Should open a write to a file when started

class LoggingComponent(object):

    def __init__(self, OutFilePath, ExpName='Dummy Experiment', ExpVer=1.0, 
                 SubjName='abc', SubjExtraInfo=str(), CondVals=dict(), StimVals=list(), 
                 Intervals='2AFC', RepCount=1, TrialCount=100, HeaderTemplateFile=None, 
                 Verbose=False, NewLineChar=None):
                     
        ''' Contructor for logging object '''
        
        self._OutFilePath = OutFilePath
        self._OutFile = None

        self._ExpName = ExpName
        self._ExpVer = ExpVer
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
        
        self._HeaderFields = None
        self._DataFields = None
        
        # TODO: check if file exists
        
        if HeaderTemplateFile != None:
            self._HeaderTemplateFile = HeaderTemplateFile
        else:
            self._HeaderTemplateFile = "default_header.txt"
        
        self._started = False
    
    def SetHeaderFields(self, *args):
        self._HeaderFields = args
    
    def SetDataFields(self, *args):
        self._DataFields = args
    
    def Start(self):
        ''' Start logging and write the header '''
        
        if not self._started:
            
            # starting timestamp
            start_timestp = datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')
            
            # open header file and get the text
            header_file = open(self._HeaderTemplateFile, 'r')
            header_text = header_file.read()
            header_file.close()
            
            if self._new_line_char != '\n':
                header_text.replace('\n', self._new_line_char)
            
            header_template = string.Template(header_text)
            
            brk_line = '='*60 # break line
            
            hdat = header_template.substitute(exp_name=self._ExpName, 
                                              exp_ver=self._ExpVer, 
                                              exp_date=start_timestp, 
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
            
            if os.path.getsize(self._OutFilePath) > 0:
                self._OutFile.write(self._new_line_char)
            
            self._OutFile.write(hdat)
            data_title_line = str()
            n = 0
            for field in self._DataFields:
                if n > 0:
                    data_title_line = '{0}    {1}'.format(data_title_line, field)
                else:
                    data_title_line = '# {0}'.format(field)
                    n += 1
            
            self._OutFile.write(data_title_line)
            self._OutFile.write(self._new_line_char)
            self._OutFile.write(self._new_line_char)
            
        else:
            
            print("ERROR: This log object is already started.")
        
    def Add(self, **kwargs):
        
        if self._started:
            
            if len(kwargs.keys()) == len(self._DataFields): # check if fields are missing
            
                # find matching indices specified by 'SetDataFields()'        
                key_map = dict()
                
                for key in kwargs.keys():
                    try:
                        key_map[self._DataFields.index(key)] = kwargs[key]
                    except KeyError:
                        print("ERROR: Invalid field name specified.")
                        return
                
                # create a sorted list of output values based on SetDataFields() indices
                out_list = list()
                
                n = 0
                while n < len(key_map.keys()):
                    out_list.append(key_map[n])
                    n += 1
                
                # write data to file
                data_field_line = str()
                n = 0
                for field in out_list:
                    if n > 0:
                        data_field_line = '{0}\t{1}'.format(data_field_line, field)
                    else:
                        data_field_line = field
                        n += 1
                
                self._OutFile.write(data_field_line)
                self._OutFile.write(self._new_line_char)
            
            else:
                raise KeyError
                print("ERROR: Field(s) missing, check if all fields are specified.")
            
        else:
            
            print("ERROR: Can not add entry, log object has not be started yet.")
        
    def Stop(self):
        
        if self._started:
            stop_timestp = datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')
            
            self._OutFile.write(self._new_line_char)
            self._OutFile.write("# {0}: Experiment ended{1}".format(stop_timestp, self._new_line_char))
                                
            self._started = False
            
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
    test_log = LoggingComponent('logging_test.txt', ExpName='Dummy Experiment',
                                CondVals={0: "INFRONT", 1: "BEHIND"}, StimVals=[0.01, 0.02, 0.05, 0.1])
    
    test_log.SetDataFields('Trial', 'Cond', 'StimLevel', 'Interv', 'Response', 'Correct', 'Latency')
    
    test_log.Start()
    test_log.Add(Trial=0, Cond=1, Interv=1, StimLevel=0.5, Response=1, Correct=1, Latency=0.3452)
    test_log.Add(Trial=1, Cond=0, Interv=2, StimLevel=0.1, Response=1, Correct=1, Latency=0.3252)
    test_log.Add(Trial=2, Cond=1, Interv=2, StimLevel=0.05, Response=0, Correct=1, Latency=0.643)
    test_log.Stop()
    
    return 0
    
if __name__ == "__main__":
    main()
