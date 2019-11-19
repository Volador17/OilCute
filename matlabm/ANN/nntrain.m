% Neural network training functions.
% Copyright (c) 1992-94 by The MathWorks, Inc.
% $Revision: 1.1 $  $Date: 1994/01/11 16:27:00 $
%
% Backpropagation network training functions.
%   trainbp  - Train feed-forward network with backpropagation.
%   trainbpx - Train feed-forward network with fast backpropagation.
%   trainlm  - Train feed-forward network with Levenberg-Marquardt.
%
% Other training functions.
%   trainp   - Train perceptron layer with perceptron rule.
%   trainpn  - Train perceptron layer with normalized perceptron rule.
%   trainwh  - Train linear layer with Widrow-Hoff rule.
%   trainelm - Train Elman recurrent network.
%   trainc   - Train competitive layer.
%   trainsm  - Train self-organizing map.
%
% Adaptive training functions.
%   adaptwh  - Adapt linear layer with Widrow-Hoff rule.
%
% See also NNINIT, NNLEARN, NNSOLVE, NNSIM, NNTRANS.

help nntrain
