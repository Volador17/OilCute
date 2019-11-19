%均值中心化程序，用于校正集外样品，如果用于校正集样品，则使用程序mcent；
% input 
% x:吸光度矩阵；
% mx：平均光谱。
% output
% xc：处理后的吸光度矩阵；
function [xc]=mcentp(x,mx)
	[m,n]=size(x);
	xc=x-mx*ones(1,n);
end