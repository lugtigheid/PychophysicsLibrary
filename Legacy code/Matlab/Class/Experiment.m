classdef Experiment < handle
    
    % Large features:
    
    % 1. Allow for random variables to be added (Uniform / Gauss)
    % 2. Data analysis incorporated into object e.g. .Analyse('fit',
    %     'none/Gaussian/Weibull/Logistic', 'method', '%correct/%answer');
    % 3. Providing feedback is important as well - perhaps.
    % 4. Variable labels to be 
    
    
    properties
        
        isFinished = false;         % Boolean to check whether we're done
        nTrials;                    % number of trials
        nRepetitions;               % number of repetitions
        trialNumber = 1;            % the current trial number (1 at start)
        Conditions = {};            % the conditions
        nIntervals = 1;             % the number of intervals
        Responses;                  % holds the observer responses
        StimulusValues;             % holds the stimulus values
        FileName = '';              % the filename to save to
        FilePath = '';              % the path to save to
        nAFC = 2;                   % the number of alternatives

        % this is where we store data after each trial
        data;
        header;
        info;
        
        % some markers that show when it started and ended
        StartDate; EndDate;
        
    end
    
    properties (Hidden)
        
        % this holds the calculated values, that we use to actually get the
        % values for each trial out of - consider array of trial objects
        
        % I decided to make this a struct because of potential conflicts
        % with existing values, that we set when initialising the class
        % (i.e. stimvals can be a small array, but that doesn't help you
        % determine the stimulus value in a random order of trials)
        
        tmpConditions;
        tmpStimvals; 
        tmpdata;
        
    end
    
    events
        
        % a simple event that triggers when we are done or abort
        Experiment_Finished;
        Experiment_Aborted;
        Give_Feedback;
        
    end
    
    methods
        
        %%
        % Constructor
        
        function obj = Experiment
            
            % add a listener to the class for when it is finished
            % when triggered, it calls the function ExperimentFinished
            obj.addlistener('Experiment_Finished', @obj.Finish);
            obj.addlistener('Experiment_Aborted', @obj.Aborted);
            
            % set the start date
            obj.StartDate = datestr(now);
            
        end
        
        %%
        % Initialisation function
        
        function obj = Initialise(obj)
            
            % initialise everything
            if isempty(obj.StimulusValues), error('Cannot initialise: Stimulus values are empty.'); end
            if isempty(obj.nRepetitions), error('Cannot initialise: No repetitions.'); end
            if isempty(obj.Conditions), error('We need at least one condition!'); end
            
            % set the number of trials
            obj.nTrials = numel(obj.StimulusValues) * numel(obj.Conditions) * obj.nRepetitions;
            
            % this is the random order of the trials
            tOrder = randperm(obj.nTrials);
            
            % this is just to provide the stimvals
            obj.tmpStimvals = repmat(obj.StimulusValues, 1, obj.nRepetitions * numel(obj.Conditions));
            
            obj.tmpConditions = repmat(obj.Conditions, obj.nRepetitions * numel(obj.StimulusValues), 1);
            
            % if this does not result in a vector, just vectorise it
            if ~isvector(obj.tmpStimvals), obj.tmpStimvals = obj.tmpStimvals(:)'; end
            
            % if this does not result in a vector, just vectorise it
            if ~isvector(obj.tmpConditions), obj.tmpConditions = obj.tmpConditions(:); end
            
            % this is the data
            obj.tmpdata.d(:,1) = 1:numel(obj.tmpStimvals);
            
            % this is the actual array of stimulus values
            obj.tmpdata.stimval = obj.tmpStimvals(tOrder);
            obj.tmpdata.condition = obj.tmpConditions(tOrder);
            
            fprintf('----\nExperiment is starting!\n----\n');
            
        end
        
        function SaveInfo(obj)
           
            % txt{1} = ['Conditions: ' for i=1:numel(obj.Conditions)];
            
            
        end
        
        function ProcessTrial(obj, trial)
            
            % put the data from the trial into the data array of the exp class 
            obj.data(trial.TrialNumber,:) = trial.GetData;
            
        end
       
        %%
        % SET Accessors
        
        function set.nRepetitions(obj,value)
            
            if value < 1,
                error('The number of repetitions has to be larger than zero!');
            else
                obj.nRepetitions = value;
            end
            
        end
        
        function set.StimulusValues(obj,value)
            
            if numel(value) < 1,
                error('There are no values in the stimulus values');
            % elseif ~isrealvec(value),
            %     error('Invalid input for stimulus values');
            else
                obj.StimulusValues = value;
            end
            
        end
        
        function set.FileName(obj,value)
            if exist([value '.mat']) > 0,
                error(['Filename (' value '.mat) already exists!']);
            else
                obj.FileName = value;
            end
            
        end
        
        %%
        % These are the procedural functions
        
        function trial = EndTrial(obj, trial)
            
            % process the trial (i.e. save into data struct)
            obj.ProcessTrial(trial);
            
            % now increment the trial number
            obj.IncrementTrialNumber();
            
            % if the trial number exceed number of trials, we finish
            if obj.trialNumber > obj.nTrials,
                notify(obj,'Experiment_Finished');
            end
        end
        
        
        function trial = StartTrial(obj)
            
            % Get a new trial object from the parameters
            % one other option is to fill an array with trial objects:
            % http://www.mathworks.com/help/techdoc/matlab_oop/brd4btr.html
            trial = Trial(obj);
            
        end
        
        function obj = IncrementTrialNumber(obj)
            
            % increments a trial number, simple
            obj.trialNumber = obj.trialNumber + 1;
            
        end
        
        % this function provides a quick overview of the data
        function data = SumData(obj, condition)
            
            % filter the conditions
            f1 = obj.data(:,2) == obj.Conditions(condition);
            
            % cycle through the stimulus values
            for iStimval = 1:numel(obj.StimulusValues),
                
                % select all trials that have this stimulus value
                f2 = obj.data(:,4) == obj.StimulusValues(iStimval);
                
                % select all trials that have a particular response (1)
                f3 = obj.data(:,5) == 1;
                
                % fill out the data array for this trial
                data(iStimval,:) = [obj.StimulusValues(iStimval) ...
                    sum(f1&f2&f3) sum(f1&f2)];
                
            end
        end
        
        
        %%
        % Closing up the object and save data
        
        function Abort(obj, varargin)
            
            % append a message so that we know it's been aborted
            obj.Filename = sprintf('ABORTED_%s', obj.Filename);
            
            % print a message
            fprintf('----\nExperiment was aborted!\n----\n');
            
            % close the object and save data
            obj.Close;
            
        end
        
        function Finish(obj, varargin)
            
            % print a message
            fprintf('----\nExperiment has finished!\n----\n');

            % close the object and save data
            obj.Close;
           
        end
        
        function Close(obj)
           
            % set the end date
            obj.EndDate = datestr(now);
            
            % set this boolean to true
            obj.isFinished = true;
            
            % save these data under a slightly different name
            Exp = obj;
            
            % save this into the data file
            save(sprintf('./%s', obj.FileName), 'Exp');
            
        end
    end
end