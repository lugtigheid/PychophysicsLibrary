#  LogManager.py
#  

import os
import sys
import datetime

class LabelError(Exception):
    def __init__(self, value):
        self.value = value
    def __str__(self):
        return "String \"{}\" contains invalid characters.".format(self.value)

class LoggingComponent(object):

    def __init__(self, OutFilePath, HeaderTemplateFile=None, Verbose=False, NewLineChar=None, CommentChar='#'):
                     
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
            
        self._CommentChar = CommentChar
        
        self._HeaderInfo = None
        self._DataFields = None
        
        # TODO: check if file exists
        
        if HeaderTemplateFile != None:
            self._HeaderTemplateFile = HeaderTemplateFile
        else:
            self._HeaderTemplateFile = "default_header.txt"
        
        self._started = False
    
    def SetHeaderInfo(self, hdat):
        self._HeaderInfo = hdat
        
        return
    
    def SetDataFields(self, *args):
        if not self._started:
            for name in args:
                if chr(32) in name:
                    raise LabelError(name)
            
            self._DataFields = args
        else:
            print("ERROR: Cannot set data fields once Start() has been called.")
    
    def _new_entry(self, data):
        self._data_registry[self._entry_idx] = data
        self._entry_idx += 1
        
        return
    
    def StartLogging(self):
        ''' Start logging and write the header '''
        
        if not self._started:
            
            if self._DataFields == None:
                
                print("ERROR: No data fields specifed for logging, can not start.")
                return # just exit
            
            # starting timestamp
            #ts_frmt = '# Experiment started at: %Y-%m-%d %H:%M:%S{}'.format(self._new_line_char)
            #self._new_entry(datetime.datetime.now().strftime(ts_frmt))

            self._started = True
            
            # open header file and get the text
            #if self._Verbose: print(hdat)
            
            #if os.path.getsize(self._OutFilePath) > 0:
                #self._OutFile.write(self._new_line_char)
            
        else:
            
            print("ERROR: This log object is already started.")
        
    def AddRecord(self, **kwargs):
        
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
    
    def GetTimeStamp(self):
        return datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        
    def StopLogging(self):
        
        if self._started:
            
            #ts_frmt = "{}# Experiment ended at: %Y-%m-%d %H:%M:%S".format(self._new_line_char)
            #self._new_entry(datetime.datetime.now().strftime(ts_frmt))

            self._started = False
            
        else:
            
            print("ERROR: This log object has not been started.")
    
    def DumpToCSVFile(self, f_path=None, header_text=None, footer_text=None, format_type='csv', 
                      fill_val='NA', no_comments=False, overwrite=False):
        
        # There are multiple format modes implimented:
        # ==================================================
        # CSV : Comma-seperatated values with no spaces (works)
        # CSV2 : Comma-seperatated values with a space
        # RCSV : Comma-seperatated values with no spaces with lables for R
        
        if self._started == False:
            
            if f_path == None: f_path = self._OutFilePath
            
            if overwrite:
                # open, blank, then close the file
                f_dat = open(f_path, 'w')
                f_dat.write('')
                f_dat.close()
            
            f_dat = open(f_path, 'a') # use append mode
            
            # write formatted header to file
            f_dat.write('{0} {1}{2}'.format(self._CommentChar, '-'*80, self._new_line_char))
            for headline in self._HeaderInfo:
                 f_dat.write('{0} {1}:\t\t{2}{3}'.format(self._CommentChar, headline[0], headline[1], self._new_line_char))
            f_dat.write('{0} {1}{2}'.format(self._CommentChar, '-'*80, self._new_line_char))
            
            # This writes basic text based formats
            if format_type.lower() in ['csv', 'rcsv', 'csv2']:
                if (format_type == 'csv') or (format_type == 'rcsv'):
                    sep_style = ','
                elif (format_type == 'csv2'):
                    sep_style = ', '
                
                # write header
                if header_text != None: f_dat.write(str(header_text))
   
                # R format has labels as a first line
                if format_type[0] == 'r':
                    col_labels_str = str()
                    n = 0
                    for field in self._DataFields:
                        if n != 0:
                            col_labels_str = "{0}{1}\"{2}\"".format(col_labels_str, 
                                sep_style, field)
                        else:
                            col_labels_str = "\"{}\"".format(field)
                            n += 1
                    
                    col_labels_str = "{0}{1}".format(col_labels_str, self._new_line_char)
                    f_dat.write(col_labels_str)
                    
                # if the file is not empty, add a newline char to space out data
                if os.path.getsize(f_path) > 0: f_dat.write(self._new_line_char)
                    
                for key, item in self._data_registry.items():
                    if isinstance(item, list):
                        data_line = str()
                        n = 0
                        for field in item:
                            if n > 0:
                                if isinstance(field, str):
                                    if format_type[0] == 'r':
                                        data_line = "{0}{1}\"{2}\"".format(data_line, sep_style, field)
                                    #else:
                                    #    data_line = "{0}{1}{2}".format(data_line, sep_style, field)
                                        
                                elif isinstance(field, int) or isinstance(field, float):
                                    data_line = "{0}{1}{2}".format(data_line, sep_style, field)
                                else:
                                    data_line = "{0}{1}{2}".format(data_line, sep_style, fill_val)
                            else:
                                data_line = field
                                n += 1
                        
                        data_line = "{0}{1}".format(data_line, self._new_line_char)
                        f_dat.write(data_line)
                        
                    else:
                        if not no_comments:
                            data_line = "{0}{1}".format(item, self._new_line_char)
                            f_dat.write(data_line)
                
                # write footer
                if footer_text != None: f_dat.write(str(footer_text))
                        
                f_dat.close()
                
            else:
                print("Unsupported formatting string, valid format strings are:")
                print("")
                print("CSV  : Comma-seperatated values with no spaces")
                print("CSV2 : Comma-seperatated values with a space")
                print("RCSV : Comma-seperatated values with no spaces + header with labels")
                print("")
                
                raise ValueError
        
        elif len(self._data_registry.keys()) == 0:
            
            print("WARNING: No log data to write!")
        
        else:
            
            print("ERROR: Log object is still active, can not write data to file until StopLogging() is called.")
            
    def __del__(self):
        pass

def main():
    # test driver
    test_log = LoggingComponent('logging_test.txt')
    
    test_log.SetHeaderInfo((('Exp Name', 'JUST TESTING'), 
                           ('Program', 'myexperiment.py'),
                           ('Version', '2014-01-10'),
                           ('Subject', 'mdc'),
                           ('StartTime', test_log.GetTimeStamp()),
                           ('Conditions', 'binocular'),
                           ('Dist', 1000)))
                           
    test_log.SetDataFields('Trial', 'Cond', 'StimLevel', 'Interv', 'Response', 'Correct', 'Latency')
    
    test_log.StartLogging()
    for i in range(50): # simulate 50 trials
        test_log.AddRecord(Trial=i, Cond=1, Interv=1, StimLevel=0.5, Response=1, Correct=1, Latency=0.3452)
        
    test_log.StopLogging()
    
    test_log.DumpToCSVFile(format_type='rcsv', no_comments=False)
    
    return 0
    
if __name__ == "__main__":
    main()
