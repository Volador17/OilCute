% Backpropagation networks.
% Copyright (c) 1992-94 by The MathWorks, Inc.
% $Revision: 1.1 $  $Date: 1994/01/11 16:23:42 $
%
% Basic functions.
%   simuff    - Simulate feed-forward network.
%   initff   - Inititialize feed-forward network.
%   trainbp  - Train feed-forward network with backpropagation.
%   trainbpx - Train feed-forward network with faster backpropagation.
%   trainelm - Train Elman recurrent network.
%   trainlm  - Train feed-forward network with Levenberg-Marquardt.
%
% Transfer functions.
%   purelin  - Hard limit transfer function.
%   tansig   - Hyperbolic tangent sigmoid transfer function.
%   logsig   - Logistic sigmoid transfer function.
%
% Delta functions.
%   deltalin - Delta function for PURELIN neurons.
%   deltatan - Delta function for TANSIG neurons.
%   deltalog - Delta function for LOGSIG neurons.
%
% Learning rules.
%   learnbp  - Backpropagation learning rule.
%   learnbpm - Backpropagation learning rule with momentum.
%   learnlm  - Levenberg-Marquardt learning rule.
%
% Analysis functions.
%   errsurf  - Error surface.
%
% Plotting functions.
%   plotes   - Plot error surface.
%   plotep   - Plot weight and bias position on error surface.
%
% Demonstrations
%   demobp1  - Training a nonlinear neuron.
%   demobp2  - Local and global error minima.
%   demobp3  - Learning with momentum.
%   demobp4  - Learning rates.
%   demobp5  - Function approximation.
%   demobp6  - Faster approximation.
%   demolm1  - Fastest approximation.
%   demolm2  - Underfitting.
%   demolm3  - Overfitting.
%   appcs1   - Nonlinear system identification.
%   appcs2   - Feedback linearization.
%   appcr1   - Character recognition.
%
% See also ELMAN.

help backprop
