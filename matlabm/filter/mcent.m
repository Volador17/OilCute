%均值中心化程序，用于校正集样品，如果用于校正集外样品，则使用程序mcentp；
% input 
% x:吸光度矩阵；
% output 
% xc：处理后的吸光度矩阵；
% mx：平均光谱。

function [xc,mx] = mcent(x)
	[m,n] = size(x);
	mx = mean(x,2);
	xc = x-mx*ones(1,n);
end