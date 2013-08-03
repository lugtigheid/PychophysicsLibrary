classdef Trial
    
    properties
        TrialNumber = NaN;
        StimulusValue = NaN;
        Condition = NaN;
        Interval = NaN;
        Response = NaN;
        RT = 0;     
        Extra = [];
    end
    
    methods
       
        
        function obj = Trial(exp)
            if nargin > 0,
                % set these values
                obj.TrialNumber = exp.trialNumber;
                obj.StimulusValue = exp.tmpdata.stimval(obj.TrialNumber);
                obj.Condition = exp.tmpdata.condition(obj.TrialNumber);
                obj.Interval = randi([1 exp.nIntervals], 1, 1);
                obj.Extra = [];
            end
        end
        
        function disp(obj)
            
            fprintf('#%.f \t %6.f \t %6.f \t %6.f \t %6.f \t %6.f \n', obj.TrialNumber, obj.Condition, ...
                obj.StimulusValue, obj.Interval, obj.Response, obj.RT);
            
        end
        
        function data = GetData(obj)
            
            data = [ ...
                obj.TrialNumber ...
                obj.Condition ...
                obj.Interval ...
                obj.StimulusValue ...
                obj.Response ...
                obj.RT ...
                obj.Extra ...
                ];
            
        end
        
    end
    
end

