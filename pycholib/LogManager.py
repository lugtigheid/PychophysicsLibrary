#  LogManager.py
#  

import os
import sys
import datetime

import string

# TODO: Should open a write to a file when started

class LoggingComponent(object):

    def __init__(self, OutFilePath, HeaderTemplateFile=None, Verbose=False, NewLineChar=None):
                     
        ''' Contructor for logging object '''
        
        self._OutFilePath = OutFilePath
        
        # we will store data lines in a dictionary
        self._data_registry = dict() # no data stored
        self._entry_idx = 0
        
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
        if not self._started:
            self._DataFields = args
        else:
            print("ERROR: Cannot set data fields once Start() has been called.")
    
    def _new_entry(self, data):
        self._data_registry[self._entry_idx] = data
        self._entry_idx += 1
        
        return
    
    def Start(self):
        ''' Start logging and write the header '''
        
        if not self._started:
            
            if self._DataFields == None:
                
                print("ERROR: No data fields specifed for logging, can not start.")
                return # just exit
            
            # starting timestamp
            ts_frmt = '# Experiment started at: %Y-%m-%d %H:%M:%S{}'.format(self._new_line_char)
            self._new_entry(datetime.datetime.now().strftime(ts_frmt))

            self._started = True
            
            # open header file and get the text
            #if self._Verbose: print(hdat)
            
            #if os.path.getsize(self._OutFilePath) > 0:
                #self._OutFile.write(self._new_line_char)
            
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
                
                # create a sorted list of output values based on SetDataFields() indices.
                # this gives some stability to the output
                out_list = list()
                
                n = 0
                while n < len(key_map.keys()):
                    out_list.append(key_map[n])
                    n += 1
                
                # add it to _data_registry
                self._new_entry(out_list)
            
            else:
                
                raise KeyError
                print("ERROR: Field(s) missing, check if all fields are specified.")
            
        else:
            
            print("ERROR: Can not add entry, log object has not be started yet.")
        
    def Stop(self):
        
        if self._started:
            
            ts_frmt = '{}# Experiment ended at: %Y-%m-%d %H:%M:%S'.format(self._new_line_char)
            self._new_entry(datetime.datetime.now().strftime(ts_frmt))

            self._started = False
            
        else:
            
            print("ERROR: This log object has not been started.")
    
    def DumpToFile(self, header_text=None, f_path=None, format='csv', overwrite=False):
        
        # There are multiple format modes to be implimented:
        # ==================================================
        # CSV : Comma-seperatated values with no spaces (works)
        # CSV2 : Comma-seperatated values with a space
        # TSV : Tab-seperated values
        # XML : Good format to store data with
        
        if f_path == None: f_path = self._OutFilePath
        
        if overwrite:
            # open, blank, then close the file
            f_dat = open(f_path, 'w')
            f_dat.write('')
            f_dat.close()
        
        f_dat = open(f_path, 'a') # use append mode
        
        #
        # TODO: Header of some sort needs to be written here.
        #   
        
        if format == 'csv':
            
            # if the file is not empty, add a newline char to space out data
            if os.path.getsize(f_path) > 0: f_dat.write(self._new_line_char)
                
            for key, item in self._data_registry.items():
                if isinstance(item, list):
                    data_line = str()
                    n = 0
                    for field in item:
                        if n > 0:
                            data_line = '{0},{1}'.format(data_line, field)
                        else:
                            data_line = field
                            n += 1
                    
                    data_line = '{0}{1}'.format(data_line, self._new_line_char)
                    
                else:
                    data_line = '{0}{1}'.format(item, self._new_line_char)
                
                f_dat.write(data_line)
            
            f_dat.close()
            
        else:
            
            # need a proper exception here
            print("ERROR: Unsupported format type.")
            
    def __del__(self):
        pass

def main():
    # test driver
    test_log = LoggingComponent('logging_test.txt')
    
    test_log.SetDataFields('Trial', 'Cond', 'StimLevel', 'Interv', 'Response', 'Correct', 'Latency')
    
    test_log.Start()
    test_log.Add(Trial=0, Cond=1, Interv=1, StimLevel=0.5, Response=1, Correct=1, Latency=0.3452)
    test_log.Add(Trial=1, Cond=0, Interv=2, StimLevel=0.1, Response=1, Correct=1, Latency=0.3252)
    test_log.Add(Trial=2, Cond=1, Response=0, Interv=2, StimLevel=0.05, Correct=1, Latency=0.643)
    test_log.Stop()
    
    test_log.DumpToFile()
    
    return 0
    
if __name__ == "__main__":
    main()
